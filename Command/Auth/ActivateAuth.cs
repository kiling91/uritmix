using System.ComponentModel;
using DataAccess.Auth;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Auth;
using Service.Security.PasswordHasher;

namespace Command.Auth;

public class ActivateAuth
{
    [DisplayName("ActivateAuth")]
    public record ActivateAuthForm
    {
        public string ConfirmCode { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string PasswordConfirm { get; init; } = null!;
    }

    [DisplayName("ActivateAuthCommand")]
    public record Command(ActivateAuthForm ActivateAuth) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.ActivateAuth).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.ActivateAuth.ConfirmCode)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.ActivateAuth.Password)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PasswordMinLength, ModelSettings.PasswordMaxLength);

                RuleFor(x => x.ActivateAuth.PasswordConfirm)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PasswordMinLength, ModelSettings.PasswordMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<Unit>>
    {
        private readonly IConfirmationCoderRepository _codeRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPersonRepository _personRepository;

        public Handler(IStringLocalizer<Handler> localizer, IConfirmationCoderRepository codeRepository,
            IPersonRepository personRepository, IPasswordHasher passwordHasher)
        {
            _codeRepository = codeRepository;
            _passwordHasher = passwordHasher;
            _personRepository = personRepository;
            _localizer = localizer;
        }

        public async Task<ResultResponse<Unit>> Handle(Command message, CancellationToken ct)
        {
            if (message.ActivateAuth.Password != message.ActivateAuth.PasswordConfirm)
                return ResultResponse<Unit>.CreateError(_localizer["Passwords do not match"]);

            var code = await _codeRepository.Find(message.ActivateAuth.ConfirmCode);
            if (code == null)
                return ResultResponse<Unit>.CreateError(_localizer["Confirm code not valid"]);

            // TODO: One transaction

            #region One transaction

            var salt = Guid.NewGuid().ToByteArray();

            var update = await _personRepository.Update(code.PersonId,
                async model =>
                {
                    var update = model with
                    {
                        Auth = new AuthModel
                        {
                            Status = AuthStatus.Activated,
                            Hash = _passwordHasher.Hash(message.ActivateAuth.Password, salt),
                            Salt = salt
                        }
                    };
                    return await update.AsTaskResult();
                });

            await _codeRepository.Remove(code.Token);

            #endregion

            return new ResultResponse<Unit>(Unit.Value);
        }
    }
}
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

public class PasswordReset
{
    [DisplayName("PasswordReset")]
    public record PasswordResetForm
    {
        public string ConfirmCode { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string PasswordConfirm { get; init; } = null!;
    }

    [DisplayName("PasswordResetCommand")]
    public record Command(PasswordResetForm PasswordReset) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.PasswordReset).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PasswordReset.ConfirmCode)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);

                RuleFor(x => x.PasswordReset.Password)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PasswordMinLength, ModelSettings.PasswordMaxLength);

                RuleFor(x => x.PasswordReset.PasswordConfirm)
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
            _localizer = localizer;
            _codeRepository = codeRepository;
            _personRepository = personRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ResultResponse<Unit>> Handle(Command message, CancellationToken ct)
        {
            if (message.PasswordReset.Password != message.PasswordReset.PasswordConfirm)
                return ResultResponse<Unit>.CreateError(_localizer["Passwords do not match"]);

            var code = await _codeRepository.Find(message.PasswordReset.ConfirmCode);
            if (code == null)
                return ResultResponse<Unit>.CreateError(_localizer["Confirm code not valid"]);
            
            var person = code.Person;
            if (person.Auth == null)
                return ResultResponse<Unit>.CreateError(_localizer["Confirm code not valid"]);
            if (person.Auth.Status == AuthStatus.Blocked)
                return ResultResponse<Unit>.CreateError(_localizer["Person is blocked"]);
            if (person.Auth.Status != AuthStatus.Activated)
                return ResultResponse<Unit>.CreateError(_localizer["Person has not activated account"]);
            
            // TODO: One transaction
            #region One transaction

            var salt = Guid.NewGuid().ToByteArray();

            await _personRepository.Update(code.PersonId,
                async model =>
                {
                    var update = model with
                    {
                        Auth = model.Auth! with
                        {
                            Hash = _passwordHasher.Hash(message.PasswordReset.Password, salt),
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
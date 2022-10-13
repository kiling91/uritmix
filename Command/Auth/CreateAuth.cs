using System.ComponentModel;
using DataAccess.Auth;
using Dto.Auth;
using FluentValidation;
using Helpers.Core;
using Helpers.Core.Extensions;
using Mapping.Enum.Person;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Auth;
using Service.Notification.SendToken;

namespace Command.Auth;

public class CreateAuth
{
    [DisplayName("CreateAuth")]
    public record CreateAuthForm
    {
        public string Email { get; init; } = null!;
        public AuthRoleView Role { get; init; }
    }

    [DisplayName("CreateAuthCommand")]
    public record Command(long PersonId, CreateAuthForm Form) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Form).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Form.Email)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength)
                    .EmailAddress();
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<Unit>>
    {
        private readonly IConfirmationCoderRepository _codeRepository;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IPersonRepository _personRepository;
        private readonly ISendToken _sendToken;

        public Handler(IPersonRepository personRepository, IStringLocalizer<Handler> localizer,
            IConfirmationCoderRepository codeRepository, ISendToken sendToken)
        {
            _personRepository = personRepository;
            _localizer = localizer;
            _codeRepository = codeRepository;
            _sendToken = sendToken;
        }

        public async Task<ResultResponse<Unit>> Handle(Command message, CancellationToken ct)
        {
            var find = await _personRepository.Get(message.PersonId);
            if (find == null)
                return ResultResponse<Unit>.CreateError(_localizer["Person not found"]);
            if (find.HaveAuth)
                return ResultResponse<Unit>.CreateError(_localizer["Person already has access to the system"]);
            var findEmail = await _personRepository.Find(message.Form.Email);
            if (findEmail != null)
                return ResultResponse<Unit>.CreateError(_localizer["Specified mail is already in use in the system"]);

            await _personRepository.Update(find.Id,
                async model =>
                {
                    var update = model with
                    {
                        HaveAuth = true,
                        Auth = new AuthModel
                        {
                            Role = message.Form.Role.ToModel(),
                            Status = AuthStatus.NotActivated,
                            Email = message.Form.Email
                        }
                    };
                    return await update.AsTaskResult();
                });


            await _codeRepository.Remove(find.Id, ConfirmTokenType.ActivatePerson);
            // Создаем код подтверждения
            var confirmationCode = await _codeRepository.Create(new ConfirmationCodeModel
            {
                PersonId = find.Id,
                Token = Guid.NewGuid().ToString(),
                Type = ConfirmTokenType.ActivatePerson,
                DateCreate = DateTime.Now
            });

            // Отправка кода подтверждения пользователя
            await _sendToken.SendActivatePersonToken(confirmationCode.Token);

            return new ResultResponse<Unit>(Unit.Value);
        }
    }
}
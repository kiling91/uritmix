using System.ComponentModel;
using DataAccess.Auth;
using FluentValidation;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Auth;
using Service.Notification.SendToken;

namespace Command.Auth;

public class PasswordResetQuery
{
    [DisplayName("PasswordResetQuery")]
    public record PasswordResetQueryForm
    {
        public string Email { get; init; } = null!;
    }

    [DisplayName("PasswordResetQueryCommand")]
    public record Command(PasswordResetQueryForm PasswordResetQuery) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.PasswordResetQuery).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.PasswordResetQuery.Email)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength);
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
            var find = await _personRepository.Find(message.PasswordResetQuery.Email);
            if (find == null)
                return ResultResponse<Unit>.CreateError(_localizer["Email not found"]);

            await _codeRepository.Remove(find.Id, ConfirmTokenType.ResetPassword);
            // Создаем код подтверждения
            var confirmationCode = await _codeRepository.Create(new ConfirmationCodeModel
            {
                PersonId = find.Id,
                Token = Guid.NewGuid().ToString(),
                Type = ConfirmTokenType.ResetPassword,
                DateCreate = DateTime.Now
            });

            // Отправка кода подтверждения пользователя
            await _sendToken.SendResetPasswordToken(confirmationCode.Token);

            return new ResultResponse<Unit>(Unit.Value);
        }
    }
}
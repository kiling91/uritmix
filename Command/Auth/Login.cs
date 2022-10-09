using System.ComponentModel;
using DataAccess.Auth;
using Dto.Auth;
using FluentValidation;
using Helpers.Core;
using Mapping.Enum.Person;
using MediatR;
using Microsoft.Extensions.Localization;
using Model;
using Model.Auth;
using Service.Security.PasswordHasher;
using Service.Security.UserJwt;

namespace Command.Auth;

public class Login
{
    [DisplayName("LoginUser")]
    public record LoginUserForm
    {
        public string Email { get; init; } = null!;
        public string Password { get; init; } = null!;
    }

    [DisplayName("LoginCommand")]
    public record Command(LoginUserForm User) : IRequest<ResultResponse<LoggedPersonView>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.User).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.User.Email)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PersonNameAndEmailMinLength, ModelSettings.PersonNameAndEmailMaxLength)
                    .EmailAddress();

                RuleFor(x => x.User.Password)
                    .NotNull()
                    .NotEmpty()
                    .Length(ModelSettings.PasswordMinLength, ModelSettings.PasswordMaxLength);
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<LoggedPersonView>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IPersonRepository _personRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Handler(IJwtTokenGenerator jwtTokenGenerator, IStringLocalizer<Handler> localizer,
            IPasswordHasher passwordHasher,
            IRefreshTokenRepository refreshTokenRepository, IPersonRepository personRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _localizer = localizer;
            _passwordHasher = passwordHasher;
            _refreshTokenRepository = refreshTokenRepository;
            _personRepository = personRepository;
        }

        public async Task<ResultResponse<LoggedPersonView>> Handle(Command message, CancellationToken ct)
        {
            var requestUser = message.User with
            {
                Email = message.User.Email.ToLower().Trim()
            };

            var person = await _personRepository.Find(requestUser.Email);
            if (person == null || person.Auth == null)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Credential error"]);
            if (person.Auth.Status == AuthStatus.Blocked)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Person is blocked"]);
            if (person.Auth.Status != AuthStatus.Activated)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Person has not activated account"]);

            var hash = _passwordHasher.Hash(requestUser.Password, person.Auth.Salt);
            if (hash != person.Auth.Hash)
                return ResultResponse<LoggedPersonView>.CreateError(_localizer["Credential error"]);

            var token = _refreshTokenRepository.CreateOrUpdate(new RefreshTokenModel
            {
                IsRevoked = false,
                PersonId = person.Id
            });

            var result = new LoggedPersonView
            {
                FirstName = person.FirstName,
                LastName = person.LastName,
                Role = person.Auth.Role.ToView(),
                Email = person.Auth.Email,
                AccessToken = _jwtTokenGenerator.CreateAccessToken(person.Id, person.Auth.Email, person.Auth.Role),
                RefreshToken = _jwtTokenGenerator.CreateRefreshToken(person.Auth.Email, token.Id)
            };

            return new ResultResponse<LoggedPersonView>(result);
        }
    }
}
using System.ComponentModel;
using DataAccess.Auth;
using FluentValidation;
using Helpers.Core;
using MediatR;
using Microsoft.Extensions.Localization;
using Model.Auth;
using Service.Security.UserJwt;

namespace Command.Auth;

public class Logout
{
    [DisplayName("Logout")]
    public record LogoutForm
    {
        public string Token { get; init; } = null!;
    }

    [DisplayName("LogoutCommand")]
    public record Command(LogoutForm Refresh) : IRequest<ResultResponse<Unit>>;

    public class CommandValidator : AbstractValidator<Command>
    {
        public CommandValidator()
        {
            RuleFor(x => x.Refresh).NotNull().DependentRules(() =>
            {
                RuleFor(x => x.Refresh.Token).NotNull().NotEmpty();
            });
        }
    }

    public class Handler : IRequestHandler<Command, ResultResponse<Unit>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IStringLocalizer<Handler> _localizer;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Handler(IRefreshTokenRepository refreshTokenRepository, IJwtTokenGenerator jwtTokenGenerator,
            IStringLocalizer<Handler> localizer)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenGenerator = jwtTokenGenerator;
            _localizer = localizer;
        }

        public async Task<ResultResponse<Unit>> Handle(Command message, CancellationToken ct)
        {
            var res = _jwtTokenGenerator.ResolveRefreshToken(message.Refresh.Token);
            if (res.Type != RefreshTokenValidateType.Valid)
                return ResultResponse<Unit>.CreateError(_localizer["Refresh token not valid"]);

            var token = await _refreshTokenRepository.Get(res.TokenId);
            if (token == null || token.Person.Auth == null)
                return ResultResponse<Unit>.CreateError(_localizer["Refresh token not valid"]);
            if (token.IsRevoked)
                return ResultResponse<Unit>.CreateError(_localizer["Refresh token has been revoked"]);

            await _refreshTokenRepository.CreateOrUpdate(new RefreshTokenModel
            {
                PersonId = token.PersonId,
                IsRevoked = true
            });
            
            return new ResultResponse<Unit>(Unit.Value);
        }
    }
}
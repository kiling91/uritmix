using Model.Auth;

namespace Service.Security.UserJwt;

public interface IJwtTokenGenerator
{
    string CreateAccessToken(long userId, string email, AuthRole role);
    string CreateRefreshToken(string email, long tokenId);
    ResolveRefreshTokenData ResolveRefreshToken(string token);
}
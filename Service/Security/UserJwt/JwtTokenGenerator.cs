using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Helpers.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Model.Auth;

namespace Service.Security.UserJwt;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly JwtOptions _jwtOptions;

    public JwtTokenGenerator(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public string CreateAccessToken(long userId, string email, AuthRole role)
    {
        var claims = new[]
        {
            new Claim(JwtClaim.Type, "access"),
            new Claim(JwtClaim.Email, email),
            new Claim(JwtClaim.Role, role.ToString()),
            new Claim(JwtClaim.UserId, userId.ToString())
            // this guarantees the token is unique
            //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var now = DateTime.UtcNow;
        var expires = now.Add(TimeSpan.FromSeconds(_jwtOptions.AccessTokenExpiresIn));

        var jwt = new JwtSecurityToken(null, null, claims, null, expires,
            JwtOptions.GetSigningCredentials(_jwtOptions.AccessTokenSecret));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public string CreateRefreshToken(string email, long tokenId)
    {
        var claims = new[]
        {
            new Claim(JwtClaim.Type, "refresh"),
            new Claim(JwtClaim.TokenId, tokenId.ToString())
        };

        var now = DateTime.UtcNow;
        var expires = now.Add(TimeSpan.FromSeconds(_jwtOptions.RefreshTokenExpiresIn));

        var jwt = new JwtSecurityToken(null, null, claims, null, expires,
            JwtOptions.GetSigningCredentials(_jwtOptions.RefreshTokenSecret));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    public ResolveRefreshTokenData ResolveRefreshToken(string token)
    {
        var res = ValidateToken(token);
        if (res != RefreshTokenValidateType.Valid)
            return new ResolveRefreshTokenData
            {
                Type = res
            };

        var handler = new JwtSecurityTokenHandler();
        var decodedValue = handler.ReadJwtToken(token);

        var tokenIdStr = decodedValue.Claims.FirstOrDefault(x => x.Type == JwtClaim.TokenId)?.Value;
        long.TryParse(tokenIdStr, out var tokenId);

        return new ResolveRefreshTokenData
        {
            TokenId = tokenId,
            Type = res
        };
    }

    private RefreshTokenValidateType ValidateToken(string authToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = JwtAuthExtensions.CreateParams(_jwtOptions.RefreshTokenSecret);
        try
        {
            tokenHandler.ValidateToken(authToken, validationParameters, out _);
            return RefreshTokenValidateType.Valid;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return RefreshTokenValidateType.Malformed;
        }
        catch (SecurityTokenExpiredException)
        {
            return RefreshTokenValidateType.Expired;
        }
        catch (SecurityTokenException)
        {
            return RefreshTokenValidateType.Malformed;
        }
        catch (ArgumentException)
        {
            return RefreshTokenValidateType.Malformed;
        }
    }
}
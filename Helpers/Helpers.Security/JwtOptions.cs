using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Helpers.Security;

public class JwtOptions
{
    public const string Options = "JwtOptions";
    public string AccessTokenSecret { get; set; } = null!;
    public string RefreshTokenSecret { get; set; } = null!;
    public int AccessTokenExpiresIn { get; set; } //seconds
    public int RefreshTokenExpiresIn { get; set; } //seconds

    public static SigningCredentials GetSigningCredentials(string authSecret)
    {
        var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSecret));
        return new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
    }
}
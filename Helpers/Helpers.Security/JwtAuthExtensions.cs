using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Helpers.Security;

public static class JwtAuthExtensions
{
    private static bool CustomLifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken tokenToValidate,
        TokenValidationParameters param)
    {
        if (expires != null) return expires > DateTime.UtcNow;
        return false;
    }

    public static TokenValidationParameters CreateParams(string authSecret)
    {
        var issuerSigningKey = JwtOptions.GetSigningCredentials(authSecret).Key;
        return new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidIssuer = null,
            ValidateAudience = false,
            ValidAudience = null,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = issuerSigningKey,
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true
            // If you want to allow a certain amount of clock drift, set that here:
            //ClockSkew = TimeSpan.Zero,
            //LifetimeValidator = CustomLifetimeValidator
        };
    }

    public static void AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtOptions = configuration.GetSection(JwtOptions.Options);
        if (jwtOptions == null)
            throw new ArgumentNullException(nameof(jwtOptions));
        var authSecret = jwtOptions.GetSection("AccessTokenSecret").Value;
        if (authSecret == null || authSecret.Length < 30)
            throw new ArgumentException("AccessTokenSecret");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = CreateParams(authSecret);
            });
    }
}
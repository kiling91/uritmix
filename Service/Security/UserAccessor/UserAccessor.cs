using Helpers.Security;
using Microsoft.AspNetCore.Http;

namespace Service.Security.UserAccessor;

public class UserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? Email()
    {
        return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == JwtClaim.Email)?.Value;
    }

    public string? Role()
    {
        return _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == JwtClaim.Role)?.Value;
    }

    public string Locale()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null && context.Request.Headers.TryGetValue("Accept-Language", out var language))
            return language;
        return "ru";
    }

    public string Ip()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context != null)
            return context.Request.Host.ToString();
        return "ru";
    }

    public long UserId()
    {
        var id = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == JwtClaim.UserId)?.Value;
        return long.Parse(id!);
    }
}
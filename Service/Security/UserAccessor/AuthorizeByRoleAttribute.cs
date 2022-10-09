using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace Service.Security.UserAccessor;

public class AuthorizeByRoleAttribute : AuthorizeAttribute
{
    public AuthorizeByRoleAttribute(params object[] roles)
    {
        AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
        if (roles.Length > 0)
        {
            Roles = "";
            foreach (var role in roles)
                Roles += role + ",";
        }
    }
}
using Dto.Auth;
using Model.Auth;

namespace Mapping.Enum.Person;

public static class MappingEnumExtensions
{
    public static AuthRoleView ToView(this AuthRole role)
    {
        switch (role)
        {
            case AuthRole.Manager:
                return AuthRoleView.Manager;
            case AuthRole.Admin:
                return AuthRoleView.Admin;
            case AuthRole.Server:
                return AuthRoleView.Server;
            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, null);
        }
    }

    public static AuthStatusView ToView(this AuthStatus status)
    {
        switch (status)
        {
            case AuthStatus.NotActivated:
                return AuthStatusView.NotActivated;
            case AuthStatus.Activated:
                return AuthStatusView.Activated;
            case AuthStatus.Blocked:
                return AuthStatusView.Blocked;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    public static AuthRole ToModel(this AuthRoleView role)
    {
        switch (role)
        {
            case AuthRoleView.Manager:
                return AuthRole.Manager;
            case AuthRoleView.Admin:
                return AuthRole.Admin;
            case AuthRoleView.Server:
                return AuthRole.Server;
            default:
                throw new ArgumentOutOfRangeException(nameof(role), role, null);
        }
    }

    public static AuthStatus ToModel(this AuthStatusView status)
    {
        switch (status)
        {
            case AuthStatusView.NotActivated:
                return AuthStatus.NotActivated;
            case AuthStatusView.Activated:
                return AuthStatus.Activated;
            case AuthStatusView.Blocked:
                return AuthStatus.Blocked;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }
}
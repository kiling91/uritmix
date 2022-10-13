using Dto.Auth;
using Dto.Person;
using Model.Auth;
using Model.Person;

namespace Mapping.Enum.Person;

public static class MappingEnumExtensions
{
    public static AuthRoleView ToView(this AuthRole value)
    {
        return (AuthRoleView)value;
    }

    public static AuthStatusView ToView(this AuthStatus value)
    {
        return (AuthStatusView)value;
    }

    public static AuthRole ToModel(this AuthRoleView value)
    {
        return (AuthRole)value;
    }

    public static AuthStatus ToModel(this AuthStatusView value)
    {
        return (AuthStatus)value;
    }

    public static PersonType ToModel(this PersonTypeView value)
    {
        return (PersonType)value;
    }
}
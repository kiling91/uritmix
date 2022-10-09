namespace Service.Security.UserAccessor;

public interface IUserAccessor
{
    string? Email();
    long UserId();
    string? Role();
    string Locale();
    string Ip();
}
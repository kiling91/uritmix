namespace Service.HostedService.InitAdmin;

public class InitAdminOptions
{
    public const string Options = "InitAdminOptions";
    public string FirstName { get; init; } = "Admin";
    public string LastName { get; init; } = "Admin";
    public string Email { get; init; } = "admin@mail.com";
    public string Password { get; init; } = "123456";
}
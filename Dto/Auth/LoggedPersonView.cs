using System.ComponentModel;

namespace Dto.Auth;

[DisplayName("LoggedPerson")]
public record LoggedPersonView
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public AuthRoleView Role { get; init; }
    public string Email { get; init; } = null!;
    public string AccessToken { get; init; } = null!;
    public string RefreshToken { get; init; } = null!;
}
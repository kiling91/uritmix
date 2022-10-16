using System.ComponentModel;

namespace View.Auth;

[DisplayName("Auth")]
public record AuthView
{
    public AuthRoleView Role { get; init; }
    public AuthStatusView Status { get; init; }
    public string Email { get; init; } = null!;
}
using System.ComponentModel;

namespace Dto.Auth;

[DisplayName("Auth")]
public record AuthView
{
    public AuthRoleView Role { get; init; }
    public AuthStatusView Status { get; init; }
    public string Email { get; init; } = null!;
}
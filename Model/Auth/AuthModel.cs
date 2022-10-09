namespace Model.Auth;

public record AuthModel
{
    public long PersonId { get; init; }
    public AuthRole Role { get; init; }
    public AuthStatus Status { get; init; }
    public string Email { get; init; } = null!;
    public string Hash { get; init; } = null!;
    public byte[] Salt { get; init; } = Array.Empty<byte>();
}
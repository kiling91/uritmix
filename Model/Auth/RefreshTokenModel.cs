using Model.Person;

namespace Model.Auth;

public record RefreshTokenModel
{
    public long Id { get; set; }
    public long PersonId { get; set; }
    public PersonModel Person { get; set; } = null!;
    public bool IsRevoked { get; set; }
}
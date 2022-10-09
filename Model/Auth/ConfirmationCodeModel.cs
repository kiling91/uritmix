using Model.Person;

namespace Model.Auth;

public record ConfirmationCodeModel
{
    public long Id { get; init; }
    public long PersonId { get; init; }
    public PersonModel Person { get; init; } = null!;
    public string Token { get; init; } = null!;
    public ConfirmTokenType Type { get; init; }
    public DateTime DateCreate { get; init; }
}
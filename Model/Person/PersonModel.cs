using Model.Auth;

namespace Model.Person;

public record PersonModel
{
    public long Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string? Description { get; init; }
    public bool IsTrainer { get; init; }
    public bool HaveAuth { get; init; }
    public AuthModel? Auth { get; init; }
}
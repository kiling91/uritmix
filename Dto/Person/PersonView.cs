using System.ComponentModel;
using Dto.Auth;

namespace Dto.Person;

[DisplayName("Person")]
public record PersonView
{
    public long Id { get; init; }
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public bool IsTrainer { get; init; }
    public string? Description { get; init; }
    public bool HaveAuth { get; init; }
    public AuthView? Auth { get; init; }
}
using Model.Person;

namespace Model.Lesson;

public record LessonModel
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
    public long TrainerId { get; init; }
    public PersonModel Trainer { get; init; } = null!;
    public int DurationMinute { get; init; }
    public float BasePrice { get; init; }
}
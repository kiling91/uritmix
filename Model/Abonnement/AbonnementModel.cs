using Model.Lesson;

namespace Model.Abonnement;

public record AbonnementModel
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public AbonnementValidity Validity { get; init; }
    public byte NumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public Discount Discount { get; init; }
    // public byte DaysOfFreezing { get; init; }
    public IEnumerable<LessonModel> Lessons { get; init; } = null!;
}
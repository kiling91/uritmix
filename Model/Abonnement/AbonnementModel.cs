using Model.Lesson;

namespace Model.Abonnement;

public record AbonnementModel
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public AbonnementValidity Validity { get; init; }
    public byte MaxNumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public Discount MaxDiscount { get; init; }
    public IEnumerable<LessonModel> Lessons { get; init; } = null!;
}
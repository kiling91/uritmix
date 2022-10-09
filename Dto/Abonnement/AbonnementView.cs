using Dto.Lesson;

namespace Dto.Abonnement;

public record AbonnementView
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public AbonnementValidityView Validity { get; init; }
    public byte NumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public DiscountView Discount { get; init; }
    // public byte DaysOfFreezing { get; init; }
    public IEnumerable<LessonView> Lessons { get; init; } = null!;
}
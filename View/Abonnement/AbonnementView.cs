using System.ComponentModel;
using View.Lesson;

namespace View.Abonnement;

[DisplayName("Abonnement")]
public record AbonnementView
{
    public long Id { get; init; }
    public string Name { get; init; } = null!;
    public AbonnementValidityView Validity { get; init; }
    public byte MaxNumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public DiscountView MaxDiscount { get; init; }
    public IEnumerable<LessonView> Lessons { get; init; } = null!;
}
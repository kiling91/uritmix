using Model.Lesson;
using Model.Person;

namespace Model.Abonnement;

public class SoldAbonnementModel
{
    public long Id { get; init; }
    public long PersonId { get; init; }
    public PersonModel Person { get; init; } = null!;
    public bool Active { get; init; }
    public DateTime DateSale { get; init; }
    public DateTime DateExpiration { get; init; }
    public float PriceSold { get; init; }
    public Discount Discount { get; init; }
    public int VisitCounter { get; init; }
    // Слепок состояния абонимента при продаже
    public string Name { get; init; } = null!;
    public AbonnementValidity Validity { get; init; }
    public byte MaxNumberOfVisits { get; init; }
    public float BasePrice { get; init; }
    public IEnumerable<LessonModel> Lessons { get; init; } = null!;
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Relational.Lesson.Entities;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Abonnement.Entities;

[Table("abonnement")]
public class AbonnementEntity : IHasId
{
    [Key] [Column("id")] public long Id { get; set; }
    [Column("name")] public string Name { get; set; } = null!;
    [Column("validity")] public byte Validity { get; set; }
    [Column("number_of_visits")] public byte MaxNumberOfVisits { get; set; }
    [Column("base_price")] public float BasePrice { get; set; }
    [Column("discount")] public byte MaxDiscount { get; set; }
    public ICollection<LessonEntity> Lessons { get; set; } = new HashSet<LessonEntity>();
}
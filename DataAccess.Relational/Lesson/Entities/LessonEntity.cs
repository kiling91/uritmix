using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Relational.Abonnement.Entities;
using DataAccess.Relational.Auth.Entities;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Lesson.Entities;

[Table("lesson")]
public class LessonEntity : IHasId
{
    [Column("name")] public string Name { get; set; } = null!;

    [Column("description")] public string? Description { get; set; }

    [Column("trainer_id")] public long TrainerId { get; set; }

    public PersonEntity Trainer { get; set; } = null!;

    [Column("duration_minute")] public int DurationMinute { get; set; }

    [Column("base_price")] public float BasePrice { get; set; }

    public IEnumerable<AbonnementEntity> Abonnements { get; set; } = null!;
    public IEnumerable<SoldAbonnementEntity> SoldAbonnements { get; set; } = null!;

    [Key] [Column("id")] public long Id { get; set; }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Relational.Relations;

[Table("sold_abonnements_lessons")]
public class SoldAbonnementsLessonsEntity
{
    [Column("abonnement_id")] public long SoldAbonnementId { get; set; }

    [Column("lesson_id")] public long LessonId { get; set; }
}
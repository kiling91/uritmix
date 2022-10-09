using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccess.Relational.Relations;

[Table("abonnements_lessons")]
public class AbonnementsLessonsEntity
{
    [Column("abonnement_id")] public long AbonnementId { get; set; }

    [Column("lesson_id")] public long LessonId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DataAccess.Relational.Lesson.Entities;
using DataAccess.Relational.Room.Entities;
using Helpers.DataAccess.Relational;

namespace DataAccess.Relational.Event.Entities;

[Table("event")]
public class EventEntry : IHasId
{
    [Key] [Column("id")] public long Id { get; set; }   
    [Column("type")] 
    public byte Type { get; init; }
    [Column("lesson_id")] public long LessonId { get; set; }
    public LessonEntity Lesson { get; set; } = null!;
    [Column("room_id")] public long RoomId { get; set; }
    public RoomEntity Room { get; set; } = null!;
    [Column("start_date")]
    public long StartDate { get; set; }
    [Column("end_date")]
    public long EndDate { get; set; }
}
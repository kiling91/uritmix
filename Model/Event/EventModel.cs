using Model.Lesson;
using Model.Room;

namespace Model.Event;

public record EventModel
{
    public long Id { get; init; }
    public EventType Type { get; init; }
    public long LessonId { get; init; }
    public LessonModel Lesson { get; init; } = null!;
    public long RoomId { get; init; }
    public RoomModel Room { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
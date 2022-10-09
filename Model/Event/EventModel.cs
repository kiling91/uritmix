using Model.Lesson;
using Model.Room;

namespace Model.Event;

public class EventModel
{
    public long Id { get; init; }
    public long LessonId { get; init; }
    public LessonModel Lesson { get; init; } = null!;
    public long RoomId { get; init; }
    public RoomModel Room { get; init; } = null!;
    public DateTime EventDateTime { get; init; }
}
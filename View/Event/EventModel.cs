using System.ComponentModel;
using View.Lesson;
using View.Room;

namespace View.Event;

[DisplayName("Event")]
public record EventView
{
    public long Id { get; init; }
    public EventTypeView Type { get; init; }
    public long LessonId { get; init; }
    public LessonView Lesson { get; init; } = null!;
    public long RoomId { get; init; }
    public RoomView Room { get; init; } = null!;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}
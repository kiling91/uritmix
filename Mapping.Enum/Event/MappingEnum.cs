using Model.Event;
using View.Event;

namespace Mapping.Enum.Event;

public static class MappingEnumExtensions
{
    public static EventTypeView ToView(this EventType value)
    {
        return (EventTypeView)value;
    }

    public static EventType ToModel(this EventTypeView value)
    {
        return (EventType)value;
    }
}
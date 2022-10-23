using Helpers.Mapping;
using Model.Event;
using Model.Lesson;
using View.Event;
using View.Lesson;

namespace Mapping.Event;

public class MappingModelToView : CustomProfile
{
    public MappingModelToView()
    {
        CreateMap<EventModel, EventView>();
    }
}
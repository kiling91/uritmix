using DataAccess.Relational.Event.Entities;
using Helpers.Mapping;
using Model.Event;

namespace Mapping.Event;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<EventModel, EventEntry>()
            //.IgnoreId()
            .Map(m => m.StartDate, m => m.StartDate.ToFileTimeUtc())
            .Map(m => m.EndDate, m => m.EndDate.ToFileTimeUtc())
            .Map(m => m.Type, m => (byte)m.Type)
            .ReverseMapExtended(this)
            .Map(m => m.StartDate, m => DateTime.FromFileTimeUtc(m.StartDate))
            .Map(m => m.EndDate, m => DateTime.FromFileTimeUtc(m.EndDate))
            .Map(m => m.Type, m => (EventType)m.Type);
    }
}
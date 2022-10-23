using DataAccess.Relational.Event.Entities;
using Helpers.Core.Extensions;
using Helpers.Mapping;
using Model.Event;

namespace Mapping.Event;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<EventModel, EventEntry>()
            .Map(m => m.StartDate, m =>   m.StartDate.ToUnixTimestamp())
            .Map(m => m.EndDate, m => m.EndDate.ToUnixTimestamp())
            .Map(m => m.Type, m => (byte)m.Type)
            .ReverseMapExtended(this)
            .Map(m => m.StartDate, m => m.StartDate.FromUnixTimestamp())
            .Map(m => m.EndDate, m => m.EndDate.FromUnixTimestamp())
            .Map(m => m.Type, m => (EventType)m.Type);
    }
}
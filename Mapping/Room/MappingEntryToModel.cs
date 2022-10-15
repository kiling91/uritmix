using DataAccess.Relational.Room.Entities;
using Helpers.Mapping;
using Model.Room;

namespace Mapping.Room;

public class MappingEntryToModel : CustomProfile
{
    public MappingEntryToModel()
    {
        CreateMap<RoomModel, RoomEntity>()
            // .IgnoreId()
            .ReverseMapExtended(this);
    }
}
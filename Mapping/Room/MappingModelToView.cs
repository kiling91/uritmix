using View.Room;
using Helpers.Mapping;
using Model.Room;

namespace Mapping.Room;

public class MappingModelToView : CustomProfile
{
    public MappingModelToView()
    {
        CreateMap<RoomModel, RoomView>();
    }
}
using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Room;

namespace DataAccess.Room;

public interface IRoomRepository
{
    Task<RoomModel> Create(RoomModel model);
    Task<UpdatedModel<RoomModel>> Update(long id, Func<RoomModel, Task<RoomModel>> updateFunc);
    Task<RoomModel?> Get(long id);
    Task<RoomModel?> Find(string name);
    Task<PaginatedList<RoomModel>> Items(Paginator paginator);
}
using AutoMapper;
using DataAccess.Relational.Room.Entities;
using DataAccess.Room;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.Extensions.Logging;
using Model.Room;

namespace DataAccess.Relational.Room;

public class RoomRepository : RepositoryBase<DbServiceContext>, IRoomRepository
{
    public RoomRepository(DbServiceContext context, IMapper map, ILogger<RoomRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<RoomModel> Create(RoomModel model)
    {
        return CreateEntity(model, c => c.Rooms);
    }

    public Task<UpdatedModel<RoomModel>> Update(long id, Func<RoomModel, Task<RoomModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.Rooms, updateFunc);
    }

    public Task<RoomModel?> Get(long id)
    {
        return GetEntity<RoomModel, RoomEntity>(e => e.Id == id, c => c.Rooms);
    }

    public Task<RoomModel?> Find(string name)
    {
        return GetEntity<RoomModel, RoomEntity>(e => e.Name == name, c => c.Rooms);
    }

    public Task<PaginatedList<RoomModel>> Items(Paginator paginator)
    {
        var query = Context.Rooms
            .OrderBy(p => p.Name);
        return PaginatedEntity<RoomModel, RoomEntity>(paginator, query);
    }
}
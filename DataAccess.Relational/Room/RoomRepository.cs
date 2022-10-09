using AutoMapper;
using DataAccess.Relational.Room.Entities;
using DataAccess.Room;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.DataAccess.Relational.Extensions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Room;

namespace DataAccess.Relational.Room;

public class RoomRepository : RepositoryBase<DbServiceContext>, IRoomRepository
{
    public RoomRepository(DbServiceContext context, IMapper mapperObject, ILogger<RoomRepository> logger)
        : base(context, mapperObject, logger)
    {
    }

    public Task<RoomModel> Create(RoomModel model)
    {
        return CreateEntity(model, context => context.Rooms);
    }

    public async Task<UpdatedModel<RoomModel>> Update(long id, Func<RoomModel, Task<RoomModel>> updateFunc)
    {
        return await UpdateEntity(
            e => e.Id == id,
            context => context.Rooms,
            updateFunc,
            (_, _) => Task.CompletedTask);
    }

    public async Task<RoomModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<RoomModel>,
            context => context.Rooms);
    }

    public async Task<RoomModel?> Find(string name)
    {
        return await GetEntity(
            e => e.Name == name,
            Dummy<RoomModel>,
            context => context.Rooms);
    }

    public async Task<PaginatedList<RoomModel>> Items(Paginator paginator)
    {
        var sessions = Context.Rooms
            .OrderBy(p => p.Name)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<RoomEntity>, PaginatedList<RoomModel>>(page);
    }
}
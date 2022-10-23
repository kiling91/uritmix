using AutoMapper;
using DataAccess.Event;
using DataAccess.Relational.Event.Entities;
using Helpers.Core.Extensions;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Event;

namespace DataAccess.Relational.Event;

public class EventRepository : RepositoryBase<DbServiceContext>, IEventRepository
{
    public EventRepository(DbServiceContext context, IMapper map, ILogger<EventRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<EventModel> Create(EventModel model)
    {
        return CreateEntity(model, c => c.Events);
    }

    public Task<UpdatedModel<EventModel>> Update(long id, Func<EventModel, Task<EventModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.Events, updateFunc);
    }

    public Task<EventModel?> Get(long id)
    {
        return GetEntity<EventModel, EventEntry>(
            e => e.Id == id,
            c => c.Events
                .Include(l => l.Lesson)
                .Include(l => l.Room)
            );
    }

    public async Task<IEnumerable<EventModel>> Items(DateTime startDate, DateTime endDate)
    {
        var start = startDate.ToUnixTimestamp();
        var end = endDate.ToUnixTimestamp();
        
        var query = Context.Events
            .Where(e => start <= e.StartDate && end >= e.StartDate)
            .Include(e => e.Room)
            .Include(e => e.Lesson)
            .ThenInclude(e => e.Trainer)
            .AsNoTracking();
        var items = await query.ToListAsync();
        return Map.Map<IEnumerable<EventEntry>, IEnumerable<EventModel>>(items);    
    }
}
using AutoMapper;
using DataAccess.Event;
using DataAccess.Lesson;
using DataAccess.Relational.Event.Entities;
using DataAccess.Relational.Lesson.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Event;
using Model.Lesson;

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

    public Task<EventModel?> Get(long id)
    {
        return GetEntity<EventModel, EventEntry>(
            e => e.Id == id,
            c => c.Events
                .Include(l => l.Lesson)
                .Include(l => l.Room)
            );
    }
}
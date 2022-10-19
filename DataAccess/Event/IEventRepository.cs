using Model.Event;

namespace DataAccess.Event;

public interface IEventRepository
{
    Task<EventModel> Create(EventModel model);
    Task<EventModel?> Get(long id);
}
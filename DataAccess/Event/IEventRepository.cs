using Helpers.DataAccess;
using Model.Event;

namespace DataAccess.Event;

public interface IEventRepository
{
    Task<EventModel> Create(EventModel model);
    Task<UpdatedModel<EventModel>> Update(long id, Func<EventModel, Task<EventModel>> updateFunc);
    Task<EventModel?> Get(long id);
    Task<IEnumerable<EventModel>> Items(DateTime startDate, DateTime endDate);
}
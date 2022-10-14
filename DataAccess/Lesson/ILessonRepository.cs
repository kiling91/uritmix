using Helpers.DataAccess;
using Helpers.Pagination;
using Model.Lesson;

namespace DataAccess.Lesson;

public interface ILessonRepository
{
    Task<LessonModel> Create(LessonModel model);
    Task<UpdatedModel<LessonModel>> Update(long id, Func<LessonModel, Task<LessonModel>> updateFunc);
    Task<LessonModel?> Get(long id);
    Task<LessonModel?> Find(string name);
    Task<List<LessonModel>> Find(IEnumerable<long> lessonsId);
    Task<PaginatedList<LessonModel>> Items(Paginator paginator);
}
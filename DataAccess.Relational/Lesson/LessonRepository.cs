using AutoMapper;
using DataAccess.Lesson;
using DataAccess.Relational.Lesson.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Lesson;

namespace DataAccess.Relational.Lesson;

public class LessonRepository : RepositoryBase<DbServiceContext>, ILessonRepository
{
    public LessonRepository(DbServiceContext context, IMapper map, ILogger<LessonRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<LessonModel> Create(LessonModel model)
    {
        return CreateEntity(model, c => c.Lessons);
    }

    public Task<UpdatedModel<LessonModel>> Update(long id, Func<LessonModel, Task<LessonModel>> updateFunc)
    {
        return UpdateEntity(e => e.Id == id, c => c.Lessons, updateFunc);
    }

    public Task<LessonModel?> Get(long id)
    {
        return GetEntity<LessonModel, LessonEntity>(
            e => e.Id == id,
            c => c.Lessons.Include(l => l.Trainer));
    }

    public Task<LessonModel?> Find(string name)
    {
        return GetEntity<LessonModel, LessonEntity>(e => e.Name == name, c => c.Lessons);
    }

    public async Task<List<LessonModel>> Find(IEnumerable<long> lessonsId)
    {
        var list = await Context.Lessons
            .Where(e => lessonsId.Contains(e.Id))
            .AsNoTracking()
            .ToListAsync();
        return Map.Map<List<LessonEntity>, List<LessonModel>>(list);
    }

    public Task<PaginatedList<LessonModel>> Items(Paginator paginator)
    {
        var query = Context.Lessons
            .Include(l => l.Trainer)
            .OrderBy(p => p.Name);
        return PaginatedEntity<LessonModel, LessonEntity>(paginator, query);
    }
}
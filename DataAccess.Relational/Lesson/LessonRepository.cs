using AutoMapper;
using DataAccess.Lesson;
using DataAccess.Relational.Lesson.Entities;
using Helpers.DataAccess;
using Helpers.DataAccess.Relational;
using Helpers.DataAccess.Relational.Extensions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Lesson;

namespace DataAccess.Relational.Lesson;

public class LessonRepository : RepositoryBase<DbServiceContext>, ILessonRepository
{
    public LessonRepository(DbServiceContext context, IMapper mapperObject, ILogger<LessonRepository> logger) : base(
        context, mapperObject, logger)
    {
    }

    public Task<LessonModel> Create(LessonModel model)
    {
        return CreateEntity(model, context => context.Lessons);
    }

    public async Task<UpdatedModel<LessonModel>> Update(long id, Func<LessonModel, Task<LessonModel>> updateFunc)
    {
        return await UpdateEntity(
            e => e.Id == id,
            context => context.Lessons,
            updateFunc,
            (_, _) => Task.CompletedTask);
    }

    public async Task<LessonModel?> Get(long id)
    {
        return await GetEntity(
            e => e.Id == id,
            Dummy<LessonModel>,
            context => context.Lessons.Include(l => l.Trainer));
    }

    public async Task<LessonModel?> Find(string name)
    {
        return await GetEntity(
            e => e.Name == name,
            Dummy<LessonModel>,
            context => context.Lessons);
    }

    public async Task<PaginatedList<LessonModel>> Items(Paginator paginator)
    {
        var sessions = Context.Lessons.Include(l => l.Trainer)
            .OrderBy(p => p.Name)
            .AsNoTracking();
        var page = await sessions.ToPaginatedListWithoutOrderingAsync(paginator);
        return MapperObject.Map<PaginatedList<LessonEntity>, PaginatedList<LessonModel>>(page);
    }
}
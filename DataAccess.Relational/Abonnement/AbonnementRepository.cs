using AutoMapper;
using DataAccess.Abonnement;
using DataAccess.Relational.Abonnement.Entities;
using DataAccess.Relational.Lesson.Entities;
using DataAccess.Relational.Relations;
using Helpers.DataAccess;
using Helpers.DataAccess.Exceptions;
using Helpers.DataAccess.Relational;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Model.Abonnement;
using Model.Lesson;

namespace DataAccess.Relational.Abonnement;

public class AbonnementRepository : RepositoryBase<DbServiceContext>, IAbonnementRepository
{
    public AbonnementRepository(DbServiceContext context, IMapper map, ILogger<AbonnementRepository> logger)
        : base(context, map, logger)
    {
    }

    public Task<AbonnementModel> Create(AbonnementModel model)
    {
        return CreateEntity(model,
            c => c.Abonnements,
            (context, entity) =>
            {
                entity.Lessons.Clear();
                foreach (var lesson in model.Lessons)
                {
                    var entityLesson = new LessonEntity { Id = lesson.Id };
                    entity.Lessons.Add(entityLesson);
                }

                context.Abonnements.Attach(entity);
                return Task.CompletedTask;
            });
    }

    public async Task<UpdatedModel<AbonnementModel>> Update(long id, Func<AbonnementModel, Task<AbonnementModel>> updateFunc)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();

        var remove = Context.AbonnementsLessons.Where(d => d.AbonnementId == id);
        Context.RemoveRange(remove);
        await Context.SaveChangesAsync();
        
        var result = await UpdateEntity(
            e => e.Id == id,
            c => c.Abonnements.Include(l => l.Lessons), 
            updateFunc, (_, entity, model) => {
                foreach (var lesson in entity.Lessons)
                {
                    var find = model.Lessons.First(l => l.Name == lesson.Name);
                    lesson.Id = find.Id;
                }

                return Task.CompletedTask;
            });
        
        await transaction.CommitAsync();
        return result;
    }

    public Task<AbonnementModel?> Get(long id)
    {
        return GetEntity<AbonnementModel, AbonnementEntity>(
            e => e.Id == id,
            c => c.Abonnements.Include(l => l.Lessons));
    }

    public Task<AbonnementModel?> Find(string name)
    {
        return GetEntity<AbonnementModel, AbonnementEntity>(
            e => e.Name == name,
            c => c.Abonnements.Include(l => l.Lessons));
    }

    public Task<PaginatedList<AbonnementModel>> Items(Paginator paginator)
    {
        var query = Context.Abonnements
            .Include(l => l.Lessons)
            .OrderBy(p => p.Name);
        return PaginatedEntity<AbonnementModel, AbonnementEntity>(paginator, query);
    }
}
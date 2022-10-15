using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Helpers.DataAccess.Exceptions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helpers.DataAccess.Relational;

public class RepositoryBase<TContext> where TContext : DbContext
{
    public RepositoryBase(TContext context, IMapper map, ILogger<RepositoryBase<TContext>> logger)
    {
        Context = context;
        Map = map;
        Logger = logger;
    }

    protected TContext Context { get; }
    protected IMapper Map { get; }
    protected ILogger<RepositoryBase<TContext>> Logger { get; }

    protected async Task<TModel> CreateEntity<TModel, TEntity>(TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor) where TEntity : class
    {
        var entity = Map.Map<TEntity>(model);
        dbSetAccessor(Context).Add(entity);
        await Context.SaveChangesAsync();
        return Map.Map<TModel>(entity);
    }

    protected async Task<TModel> CreateEntity<TModel, TEntity>(
        TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Func<TContext, TEntity, Task> updateRelatedEntitiesTask
    ) where TEntity : class
    {
        var entity = Map.Map<TEntity>(model);
        // При необходимости - обновляем связанные сущности
        await updateRelatedEntitiesTask.Invoke(Context, entity);

        dbSetAccessor(Context).Add(entity);
        await Context.SaveChangesAsync();
        return Map.Map<TModel>(entity);
    }

    protected async Task<TModel?> GetEntity<TModel, TEntity>(Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor) where TEntity : class
    {
        var entity = await dbSetAccessor(Context).AsNoTracking().FirstOrDefaultAsync(entitySelector);
        return Map.Map<TModel?>(entity);
    }

    protected async Task<TModel> CreateOrUpdateEntity<TModel, TEntity>(TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Expression<Func<TEntity, bool>> entitySelector)
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);

        if (entity == null)
        {
            entity = Map.Map<TEntity>(model);
            dbSetAccessor(Context).Add(entity);
        }
        else
        {
            Map.Map(model, entity);
        }

        await Context.SaveChangesAsync();

        return Map.Map<TModel>(entity);
    }

    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask
    )
    {
        return UpdateEntity(entitySelector, dbSetAccessor, getUpdatedModelTask, (_, _, _) => Task.CompletedTask);
    }

    protected async Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask,
        Func<TContext, TEntity, TModel, Task> handleRelatedEntitiesTask
    )
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null)
            throw new EntityDoesNotExistsException<TEntity>();

        var oldModel = Map.Map<TModel>(entity);
        var model = Map.Map<TModel>(entity);

        model = await getUpdatedModelTask(model);
        Map.Map(model, entity);
        await handleRelatedEntitiesTask(Context, entity, model);

        await Context.SaveChangesAsync();
        return new UpdatedModel<TModel>(Map.Map<TModel>(entity), oldModel);
    }

    protected async Task<TModel> DeleteEntity<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, DbSet<TEntity>> dbSetAccessor) where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null)
            throw new EntityDoesNotExistsException<TEntity>();

        var model = Map.Map<TModel>(entity);

        dbSetAccessor(Context).Remove(entity);
        await Context.SaveChangesAsync();

        return model;
    }

    protected Task<PaginatedList<TModel>> PaginatedEntity<TModel, TEntity>(
        Paginator paginator,
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor
    ) where TEntity : class
    {
        var query = dbSetAccessor(Context).Where(entitySelector);
        return PaginatedEntity<TModel, TEntity>(paginator, query);
    }

    protected async Task<PaginatedList<TModel>> PaginatedEntity<TModel, TEntity>(
        Paginator paginator,
        IQueryable<TEntity> query
    ) where TEntity : class
    {
        query = query.AsNoTracking();
        var count = await query.CountAsync();
        var items = await query.Skip((paginator.PageNumber - 1) * paginator.PageSize)
            .Take(paginator.PageSize).ToListAsync();

        var page = new PaginatedList<TEntity>(items, count, paginator.PageSize, paginator.PageNumber);
        return Map.Map<PaginatedList<TEntity>, PaginatedList<TModel>>(page);
    }
}
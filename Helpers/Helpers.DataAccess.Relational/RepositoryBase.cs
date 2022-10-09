using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Helpers.Core.Extensions;
using Helpers.DataAccess.Exceptions;
using Helpers.DataAccess.Relational.Extensions;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Helpers.DataAccess.Relational;

/// <summary>
///     Repository base class
/// </summary>
/// <typeparam name="TContext">The type of the context.</typeparam>
public class RepositoryBase<TContext>
    where TContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="RepositoryBase{TContext}" /> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="mapperObject">The mapper.</param>
    /// <param name="logger">The logger.</param>
    public RepositoryBase(TContext context, IMapper mapperObject, ILogger<RepositoryBase<TContext>> logger)
    {
        Context = context;
        MapperObject = mapperObject;
        Logger = logger;
    }

    /// <summary>
    ///     Gets the db context.
    /// </summary>
    protected TContext Context { get; }

    /// <summary>
    ///     Gets the mapper.
    /// </summary>
    protected virtual IMapper MapperObject { get; }

    /// <summary>
    ///     Gets the logger.
    /// </summary>
    protected ILogger<RepositoryBase<TContext>> Logger { get; }

    /// <summary>
    ///     Создает сущность по модели
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="model">Модель с данными для создания</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="alreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>Модель созданной сущности</returns>
    protected Task<TModel> CreateEntity<TModel, TEntity>(
        TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor
    )
        where TEntity : class
    {
        var alreadyExistsException = new EntityAlreadyExistsException<TEntity>();
        return CreateEntity(model, dbSetAccessor, (context, entity) => Task.CompletedTask, alreadyExistsException);
    }

    /// <summary>
    ///     Создает сущность по модели
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="model">Модель с данными для создания</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="updateRelatedEntitiesTask">Функция для обновления связанных сущностей</param>
    /// <param name="alreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Модель созданной сущности
    /// </returns>
    protected async Task<TModel> CreateEntity<TModel, TEntity, TEntityAlreadyExistsException>(
        TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Func<TContext, TEntity, Task> updateRelatedEntitiesTask,
        TEntityAlreadyExistsException alreadyExistsException
    )
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        // Создаем сущность для сохранения
        var entity = MapperObject.Map<TEntity>(model);

        // При необходимости - обновляем связанные сущности
        await updateRelatedEntitiesTask.Invoke(Context, entity);

        try
        {
            // Добавляем сущность в контекст и сохраняем изменения
            dbSetAccessor(Context).Add(entity);

            await Context.SaveChangesAsync();

            // Возвращаем модель созданной сущности
            return MapperObject.Map<TModel>(entity);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
        {
            var serializedEntity = SerializeWithoutErrors(entity);
            Logger.LogWarning($"Failed to perform CreateEntity operation for entity {serializedEntity}: {ex}");

            Logger.LogDebug("Removing failed entity from db context");
            dbSetAccessor(Context).Remove(entity);

            throw alreadyExistsException;
        }
    }

    /// <summary>
    ///     Создает сущность по модели
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="models">Модели с данными для создания</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="alreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>Модель созданной сущности</returns>
    protected Task<List<TModel>> CreateEntities<TModel, TEntity, TEntityAlreadyExistsException>(
        IEnumerable<TModel> models,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        TEntityAlreadyExistsException alreadyExistsException
    )
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        return CreateEntities(models, dbSetAccessor, (context, entity) => Task.CompletedTask, alreadyExistsException);
    }

    /// <summary>
    ///     Создает сущность по модели
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="models">Модели с данными для создания</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="updateRelatedEntitiesTask">Функция для обновления связанных сущностей</param>
    /// <param name="alreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Модель созданной сущности
    /// </returns>
    protected async Task<List<TModel>> CreateEntities<TModel, TEntity, TEntityAlreadyExistsException>(
        IEnumerable<TModel> models,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Func<TContext, TEntity, Task> updateRelatedEntitiesTask,
        TEntityAlreadyExistsException alreadyExistsException
    )
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        // Создаем сущность для сохранения
        var entities = models.Select(model => MapperObject.Map<TEntity>(model)).ToList();

        // При необходимости - обновляем связанные сущности
        foreach (var entity in entities) await updateRelatedEntitiesTask.Invoke(Context, entity);

        try
        {
            // Добавляем сущность в контекст и сохраняем изменения
            dbSetAccessor(Context).AddRange(entities);

            await Context.SaveChangesAsync();

            // Возвращаем модель созданной сущности
            return entities.Select(entity => MapperObject.Map<TModel>(entity)).ToList();
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
        {
            var serializedEntities = SerializeWithoutErrors(entities);
            Logger.LogWarning($"Failed to perform CreateEntity operation for entities {serializedEntities}: {ex}");
            throw alreadyExistsException;
        }
    }

    /// <summary>
    ///     Creates  or updates the selected entity.
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <param name="model">Модель с данными для создания</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <returns>Entity model</returns>
    protected async Task<TModel> CreateOrUpdateEntity<TModel, TEntity>(TModel model,
        Func<TContext, DbSet<TEntity>> dbSetAccessor, Expression<Func<TEntity, bool>> entitySelector)
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);

        if (entity == null)
        {
            entity = MapperObject.Map<TEntity>(model);
            dbSetAccessor(Context).Add(entity);
        }
        else
        {
            MapperObject.Map(model, entity);
        }

        await Context.SaveChangesAsync();

        return MapperObject.Map<TModel>(entity);
    }

    /// <summary>
    ///     Получает модель по ее Id
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entityId">Id cущности</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Entity model</returns>
    protected Task<TModel> GetEntity<TModel, TEntity>(long entityId, Func<TModel> dummy,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor, Exception entityNotFoundException)
        where TEntity : class, IHasId
    {
        return GetEntity(e => e.Id == entityId, dummy, dbSetAccessor, entityNotFoundException);
    }

    /// <summary>
    ///     Получает модель по ее Id с отслеживанием изменений
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entityId">Id cущности</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Entity model</returns>
    protected Task<TModel> GetEntityWithTracking<TModel, TEntity>(long entityId, Func<TModel> dummy,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor, Exception entityNotFoundException)
        where TEntity : class, IHasId
    {
        return GetEntityWithTracking(e => e.Id == entityId, dummy, dbSetAccessor, entityNotFoundException);
    }

    /// <summary>
    ///     Gets the entities.
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <returns>Entity models</returns>
    protected async Task<List<TModel>> GetEntities<TModel, TEntity>(Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy, Func<TContext, IQueryable<TEntity>> dbSetAccessor)
        where TEntity : class
    {
        var entities = await dbSetAccessor(Context).AsNoTracking().Where(entitySelector).ToListAsync();

        return MapperObject.Map<List<TModel>>(entities);
    }

    /// <summary>
    ///     Gets the entities.
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="paginator">The paginator.</param>
    /// <returns>Entity models</returns>
    protected async Task<PaginatedList<TModel>> GetEntitiesPage<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy, Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Paginator paginator)
        where TEntity : class, IHasId
    {
        var entities = await dbSetAccessor(Context).AsNoTracking().Where(entitySelector)
            .ToPaginatedListAsync(paginator);

        return MapperObject.Map<PaginatedList<TModel>>(entities);
    }

    /// <summary>
    ///     Получает модель по ее Id
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Entity model</returns>
    protected async Task<TModel> GetEntity<TModel, TEntity>(Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy, Func<TContext, IQueryable<TEntity>> dbSetAccessor, Exception entityNotFoundException)
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).AsNoTracking().FirstOrDefaultAsync(entitySelector);
        if (entity == null && entityNotFoundException != null)
            throw entityNotFoundException;

        return MapperObject.Map<TModel>(entity);
    }

    /// <summary>
    ///     Получает модель по ее Id
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <returns>Entity model</returns>
    protected async Task<TModel> GetEntity<TModel, TEntity>(Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy, Func<TContext, IQueryable<TEntity>> dbSetAccessor)
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).AsNoTracking().FirstOrDefaultAsync(entitySelector);
        return MapperObject.Map<TModel>(entity);
    }


    /// <summary>
    ///     Получает модель по ее Id с отслеживанием изменений
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Entity model</returns>
    protected async Task<TModel> GetEntityWithTracking<TModel, TEntity>(Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy, Func<TContext, IQueryable<TEntity>> dbSetAccessor, Exception entityNotFoundException)
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null && entityNotFoundException != null)
            throw entityNotFoundException;

        return MapperObject.Map<TModel>(entity);
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="entityId">Id cущности</param>
    /// <param name="model">Модель с данными для обновления</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <param name="entityAlreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity, TEntityNotFoundException,
        TEntityAlreadyExistsException>(
        long entityId,
        TModel model,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        TEntityNotFoundException entityNotFoundException,
        TEntityAlreadyExistsException entityAlreadyExistsException = default
    )
        where TEntityNotFoundException : Exception
        where TEntityAlreadyExistsException : Exception
        where TEntity : class, IHasId
    {
        return UpdateEntity(e => e.Id == entityId, model, dbSetAccessor, (context, entity) => Task.CompletedTask,
            entityNotFoundException, entityAlreadyExistsException);
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <param name="entityId">Id cущности</param>
    /// <param name="model">Модель с данными для обновления</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity, TEntityNotFoundException>(
        long entityId,
        TModel model,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        TEntityNotFoundException entityNotFoundException
    )
        where TEntityNotFoundException : Exception
        where TEntity : class, IHasId
    {
        return UpdateEntity(e => e.Id == entityId, model, dbSetAccessor, (context, entity) => Task.CompletedTask,
            entityNotFoundException, (Exception)null);
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="model">Модель с данными для обновления</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <param name="entityAlreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity, TEntityNotFoundException,
        TEntityAlreadyExistsException>(
        Expression<Func<TEntity, bool>> entitySelector,
        TModel model,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        TEntityNotFoundException entityNotFoundException,
        TEntityAlreadyExistsException entityAlreadyExistsException = default
    )
        where TEntityNotFoundException : Exception
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        return UpdateEntity(entitySelector, model, dbSetAccessor, (context, entity) => Task.CompletedTask,
            entityNotFoundException, entityAlreadyExistsException);
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="model">Модель с данными для обновления</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="handleRelatedEntitesTask">Функция для обработки связанных сущностей</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <param name="entityAlreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity, TEntityNotFoundException,
        TEntityAlreadyExistsException>(
        Expression<Func<TEntity, bool>> entitySelector,
        TModel model,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Func<TContext, TEntity, Task> handleRelatedEntitesTask,
        TEntityNotFoundException entityNotFoundException,
        TEntityAlreadyExistsException entityAlreadyExistsException = default
    )
        where TEntityNotFoundException : Exception
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        return UpdateEntity<TModel, TEntity, TEntityNotFoundException, TEntityAlreadyExistsException>(
            entitySelector, dbSetAccessor, oldModel => model.AsTaskResult(), handleRelatedEntitesTask,
            entityNotFoundException, entityAlreadyExistsException
        );
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="getUpdatedModelTask">Функция для получения обновленной модели из существующей</param>
    /// <param name="handleRelatedEntitesTask">Функция для обработки связанных сущностей</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <param name="entityAlreadyExistsException">Исключение, которое должно быть выброшено, если сущность уже существует</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected async Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity, TEntityNotFoundException,
        TEntityAlreadyExistsException>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask,
        Func<TContext, TEntity, Task> handleRelatedEntitesTask,
        TEntityNotFoundException entityNotFoundException,
        TEntityAlreadyExistsException entityAlreadyExistsException = default
    )
        where TEntityNotFoundException : Exception
        where TEntityAlreadyExistsException : Exception
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null)
            throw entityNotFoundException;

        var oldModel = MapperObject.Map<TModel>(entity);

        var model = MapperObject.Map<TModel>(entity);
        model = await getUpdatedModelTask(model);
        MapperObject.Map(model, entity);

        await handleRelatedEntitesTask(Context, entity);

        try
        {
            await Context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
        {
            var serializedEntity = SerializeWithoutErrors(entity);
            Logger.LogWarning($"Failed to perform UpdateEntity operation for entities {serializedEntity}: {ex}");
            throw entityAlreadyExistsException ?? ex;
        }

        return new UpdatedModel<TModel>(MapperObject.Map<TModel>(entity), oldModel);
    }

    /// <summary>
    ///     Обновляет модель в базе данных
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность не найдена</typeparam>
    /// <typeparam name="TEntityAlreadyExistsException">
    ///     Тип исключения, которое должно быть выброшено, если сущность уже
    ///     существует
    /// </typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="getUpdatedModelTask">Функция для получения обновленной модели из существующей</param>
    /// <param name="handleRelatedEntitesTask">Функция для обработки связанных сущностей</param>
    /// <returns>
    ///     Обновленная модель
    /// </returns>
    protected Task<UpdatedModel<TModel>> UpdateEntity<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TContext, IQueryable<TEntity>> dbSetAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask,
        Func<TContext, TEntity, Task> handleRelatedEntitesTask
    )
        where TEntity : class
    {
        return UpdateEntity(
            entitySelector, dbSetAccessor, getUpdatedModelTask, handleRelatedEntitesTask,
            new EntityDoesNotExistsException<TModel>(), new EntityAlreadyExistsException<TModel>());
    }


    /// <summary>
    ///     Удаляет сущность из базы
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность уже существует</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Модель с данынми удаленной сущности</returns>
    protected async Task<TModel> DeleteEntity<TModel, TEntity, TEntityNotFoundException>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        TEntityNotFoundException entityNotFoundException
    )
        where TEntityNotFoundException : Exception
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null)
            throw entityNotFoundException;

        var model = MapperObject.Map<TModel>(entity);

        dbSetAccessor(Context).Remove(entity);
        await Context.SaveChangesAsync();

        return model;
    }

    /// <summary>
    ///     Удаляет сущность из базы
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="throwDoesNotExistsException"></param>
    protected async Task<TModel> DeleteEntity<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        bool throwDoesNotExistsException = true
    )
        where TModel : class
        where TEntity : class
    {
        var entity = await dbSetAccessor(Context).FirstOrDefaultAsync(entitySelector);
        if (entity == null)
        {
            if (throwDoesNotExistsException)
                throw new EntityDoesNotExistsException<TModel>();
            return null;
        }

        var model = MapperObject.Map<TModel>(entity);

        dbSetAccessor(Context).Remove(entity);
        await Context.SaveChangesAsync();

        return model;
    }

    /// <summary>
    ///     Удаляет сущности из базы
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <returns>Модели с данынми удаленных сущностей</returns>
    protected async Task<List<TModel>> DeleteEntities<TModel, TEntity>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy,
        Func<TContext, DbSet<TEntity>> dbSetAccessor)
        where TEntity : class
    {
        var entities = await dbSetAccessor(Context).Where(entitySelector).ToListAsync();

        var models = MapperObject.Map<List<TModel>>(entities);

        dbSetAccessor(Context).RemoveRange(entities);
        await Context.SaveChangesAsync();

        return models;
    }

    /// <summary>
    ///     Удаляет сущность из базы
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность уже существует</typeparam>
    /// <param name="entitySelector">Условие для поиска сущности в БД</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="includes">List of includes, that should be applied to DbSet</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Модель с данынми удаленной сущности</returns>
    protected async Task<TModel> DeleteEntity<TModel, TEntity, TEntityNotFoundException>(
        Expression<Func<TEntity, bool>> entitySelector,
        Func<TModel> dummy,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Func<DbSet<TEntity>, IQueryable<TEntity>> includes,
        TEntityNotFoundException entityNotFoundException
    )
        where TEntityNotFoundException : Exception
        where TEntity : class
    {
        var entities = includes != null ? includes(dbSetAccessor(Context)) : dbSetAccessor(Context);
        var entity = await entities.FirstOrDefaultAsync(entitySelector);
        if (entity == null)
            throw entityNotFoundException;

        var model = MapperObject.Map<TModel>(entity);

        dbSetAccessor(Context).Remove(entity);
        await Context.SaveChangesAsync();

        return model;
    }

    /// <summary>
    ///     Удаляет сущность из базы
    /// </summary>
    /// <typeparam name="TModel">Тип получаемой модели</typeparam>
    /// <typeparam name="TEntity">Тип сущности в БД</typeparam>
    /// <typeparam name="TEntityNotFoundException">Тип исключения, которое должно быть выброшено, если сущность уже существует</typeparam>
    /// <param name="entityId">Id cущности</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entityNotFoundException">Исключение, которое должно быть выброшено, если сущность не найдена</param>
    /// <returns>Модель с данынми удаленной сущности</returns>
    protected Task<TModel> DeleteEntity<TModel, TEntity, TEntityNotFoundException>(
        long entityId,
        Func<TModel> dummy,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        TEntityNotFoundException entityNotFoundException
    )
        where TEntityNotFoundException : Exception
        where TEntity : class, IHasId
    {
        return DeleteEntity(e => e.Id == entityId, dummy, dbSetAccessor, entityNotFoundException);
    }

    /// <summary>
    ///     Создает элемент в дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TElementAlreadyExistsException">
    ///     Тип исключения, которое будет выброшего, если элемент уже существует в
    ///     коллекции
    /// </typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="createModel">Модель для добавления в коллекцию</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="rootNotFoundException">
    ///     Исключение, которое следует выбросить, если родительская сущность не найдена в
    ///     DbSet'е
    /// </param>
    /// <param name="elementAlreadyExistsException">
    ///     Исключение, которое следует выбросить, если элемент уже существует в
    ///     коллекции
    /// </param>
    /// <returns>Модель добавленного элемента</returns>
    protected Task<TModel> CreateChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TElementAlreadyExistsException>(
        long rootId,
        TModel createModel,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException rootNotFoundException,
        TElementAlreadyExistsException elementAlreadyExistsException
    )
        where TRootNotFoundException : Exception
        where TElementAlreadyExistsException : Exception
        where TRootEntity : class, IHasId
    {
        return CreateChildCollectionItem(e => e.Id == rootId, createModel, dbSetAccessor, collectionAccessor,
            rootNotFoundException, elementAlreadyExistsException);
    }

    /// <summary>
    ///     Создает элемент в дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TElementAlreadyExistsException">
    ///     Тип исключения, которое будет выброшего, если элемент уже существует в
    ///     коллекции
    /// </typeparam>
    /// <param name="rootSelector">Условие для поиска родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="createModel">Модель для добавления в коллекцию</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="rootNotFoundException">
    ///     Исключение, которое следует выбросить, если родительская сущность не найдена в
    ///     DbSet'е
    /// </param>
    /// <param name="elementAlreadyExistsException">
    ///     Исключение, которое следует выбросить, если элемент уже существует в
    ///     коллекции
    /// </param>
    /// <returns>Модель добавленного элемента</returns>
    protected async Task<TModel> CreateChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TElementAlreadyExistsException>(
        Expression<Func<TRootEntity, bool>> rootSelector,
        TModel createModel,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException rootNotFoundException,
        TElementAlreadyExistsException elementAlreadyExistsException
    )
        where TRootNotFoundException : Exception
        where TElementAlreadyExistsException : Exception
        where TRootEntity : class
    {
        // Находим родительский элемент, включив в него дочернюю коллекцию с помощью Include
        var rootEntity = await dbSetAccessor(Context).Include(collectionAccessor).FirstOrDefaultAsync(rootSelector);
        if (rootEntity == null)
            throw rootNotFoundException; // Если не нашли, выбрасываем исключение

        // Создаем сущность из модели
        var newEntity = MapperObject.Map<TEntity>(createModel);
        var logEntity = MapperObject.Map<TEntity>(createModel);
        try
        {
            // Добавляем созданную сущность ее в дочернюю коллекцию
            collectionAccessor.Compile().Invoke(rootEntity).Add(newEntity);

            // Сохраняем добавленный элемент (тут же он получает Id и т.п.)
            await Context.SaveChangesAsync();

            // Возвращаем модель добавленного элемента
            return MapperObject.Map<TModel>(newEntity);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
        {
            var serializedEntity = SerializeWithoutErrors(logEntity);
            var serializedModel = SerializeWithoutErrors(createModel);
            Logger.LogWarning(
                $"Error during CreateChildEntity operation for entity {serializedEntity} from model {serializedModel}: {ex}");
            throw elementAlreadyExistsException;
        }
    }

    /// <summary>
    ///     Удаляет элемент из дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TItemNotFoundException">Тип исключения, которое будет выброшего, если элемент для удаления не найден</typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="itemId">Id элемента дочерней коллекции для удаления</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="rootNotFoundException">
    ///     Исключение, которое следует выбросить, если родительская сущность не найдена в
    ///     DbSet'е
    /// </param>
    /// <param name="itemNotFoundException">
    ///     Исключение, которое следует выбросить, если элемент для удаления не найден в
    ///     коллекции
    /// </param>
    /// <returns>Модель добавленного элемента</returns>
    protected Task<TModel> DeleteChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TItemNotFoundException>(
        long rootId,
        long itemId,
        Func<TModel> dummy,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException rootNotFoundException,
        TItemNotFoundException itemNotFoundException
    )
        where TRootNotFoundException : Exception
        where TItemNotFoundException : Exception
        where TRootEntity : class, IHasId
        where TEntity : IHasId
    {
        return DeleteChildCollectionItem(rootId, e => e.Id == itemId, dummy, dbSetAccessor, collectionAccessor,
            rootNotFoundException, itemNotFoundException);
    }

    /// <summary>
    ///     Удаляет элемент из дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TItemNotFoundException">Тип исключения, которое будет выброшего, если элемент для удаления не найден</typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="itemSelector">Предикат для поиска элемента дочерней коллекции для удаления</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="rootNotFoundException">
    ///     Исключение, которое следует выбросить, если родительская сущность не найдена в
    ///     DbSet'е
    /// </param>
    /// <param name="itemNotFoundException">
    ///     Исключение, которое следует выбросить, если элемент для удаления не найден в
    ///     коллекции
    /// </param>
    /// <returns>Модель добавленного элемента</returns>
    protected async Task<TModel> DeleteChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TItemNotFoundException>(
        long rootId,
        Predicate<TEntity> itemSelector,
        Func<TModel> dummy,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException rootNotFoundException,
        TItemNotFoundException itemNotFoundException
    )
        where TRootNotFoundException : Exception
        where TItemNotFoundException : Exception
        where TRootEntity : class, IHasId
    {
        // Находим родительский элемент, включив в него дочернюю коллекцию с помощью Include
        var rootEntity = await dbSetAccessor(Context).Include(collectionAccessor)
            .FirstOrDefaultAsync(e => e.Id == rootId);
        if (rootEntity == null)
            throw rootNotFoundException; // Если не нашли, выбрасываем исключение

        // Находим элемент для удаления
        var entityToDelete = collectionAccessor.Compile().Invoke(rootEntity).Find(itemSelector);
        if (entityToDelete == null)
            throw itemNotFoundException; // Если не нашли, выбрасываем исключение

        // Сохраняем модель до удаления
        var viewModel = MapperObject.Map<TModel>(entityToDelete);

        // Удаляем найденный элемент
        collectionAccessor.Compile().Invoke(rootEntity).Remove(entityToDelete);

        // Сохраняем обновленную коллекцию
        await Context.SaveChangesAsync();

        // Возвращаем модель удаленного элемента
        return viewModel;
    }

    /// <summary>
    ///     Обновляет элемент в дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// ///
    /// <typeparam name="TChildEntityNotFoundException">
    ///     Тип исключения, которое будет выброшено, если дочерняя сущность не
    ///     найдена
    /// </typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="itemId">Id элемента дочерней коллекции для обновления</param>
    /// <param name="updateModel">Модель с данными для обновления элемента коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="exception">Исключение, которое следует выбросить, если родительская сущность не найдена в DbSet'е</param>
    /// ///
    /// <param name="childNotFoundException">Исключение, которое будет выброшено, если дочерняя сущность не найдена</param>
    /// <returns>Модель обновленного элемента</returns>
    protected Task<UpdatedModel<TModel>> UpdateChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TChildEntityNotFoundException>(
        long rootId,
        long itemId,
        TModel updateModel,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException exception,
        TChildEntityNotFoundException childNotFoundException = null
    )
        where TRootNotFoundException : EntityDoesNotExistsException
        where TChildEntityNotFoundException : EntityDoesNotExistsException
        where TRootEntity : class, IHasId
        where TEntity : IHasId
    {
        return UpdateChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
            TChildEntityNotFoundException>(
            rootId, itemId, dbSetAccessor, collectionAccessor, oldModel => updateModel.AsTaskResult(), exception,
            childNotFoundException
        );
    }

    /// <summary>
    ///     Обновляет элемент в дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TChildEntityNotFoundException">
    ///     Тип исключения, которое будет выброшено, если дочерняя сущность не
    ///     найдена
    /// </typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="itemId">Id элемента дочерней коллекции для обновления</param>
    /// <param name="getUpdatedModelTask">Задача для получения модели с данными для обновления элемента коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="exception">Исключение, которое следует выбросить, если родительская сущность не найдена в DbSet'е</param>
    /// <param name="childNotFoundException">Исключение, которое будет выброшено, если дочерняя сущность не найдена</param>
    /// <returns>Модель обновленного элемента</returns>
    protected Task<UpdatedModel<TModel>> UpdateChildCollectionItem<TModel, TEntity, TRootEntity, TRootNotFoundException,
        TChildEntityNotFoundException>(
        long rootId,
        long itemId,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask,
        TRootNotFoundException exception,
        TChildEntityNotFoundException childNotFoundException
    )
        where TRootNotFoundException : Exception
        where TChildEntityNotFoundException : Exception
        where TRootEntity : class, IHasId
        where TEntity : IHasId
    {
        return UpdateChildCollectionItem(rootId, e => e.Id == itemId, dbSetAccessor, collectionAccessor,
            getUpdatedModelTask, exception, childNotFoundException);
    }

    /// <summary>
    ///     Обновляет элемент в дочерней коллекции
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <typeparam name="TChildEntityNotFoundException">
    ///     Тип исключения, которое будет выброшено, если дочерняя сущность не
    ///     найдена
    /// </typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="itemSelector">Предикат выбора элемента дочерней коллекции для обновления</param>
    /// <param name="getUpdatedModelTask">Задача для получения модели с данными для обновления элемента коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="exception">Исключение, которое следует выбросить, если родительская сущность не найдена в DbSet'е</param>
    /// <param name="childNotFoundException">Исключение, которое будет выброшено, если дочерняя сущность не найдена</param>
    /// <returns>Модель обновленного элемента</returns>
    protected async Task<UpdatedModel<TModel>> UpdateChildCollectionItem<TModel, TEntity, TRootEntity,
        TRootNotFoundException, TChildEntityNotFoundException>(
        long rootId,
        Predicate<TEntity> itemSelector,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        Func<TModel, Task<TModel>> getUpdatedModelTask,
        TRootNotFoundException exception,
        TChildEntityNotFoundException childNotFoundException
    )
        where TRootNotFoundException : Exception
        where TChildEntityNotFoundException : Exception
        where TRootEntity : class, IHasId
    {
        // Находим родительский элемент, включив в него дочернюю коллекцию с помощью Include
        var rootEntity = await dbSetAccessor(Context).Include(collectionAccessor)
            .FirstOrDefaultAsync(e => e.Id == rootId);
        if (rootEntity == null)
            throw exception; // Если не нашли, выбрасываем исключение

        // Находим сущность для обновления, сохраняем старую модель и обновляем сущность
        var entityToUpdate = collectionAccessor.Compile().Invoke(rootEntity).Find(itemSelector);
        if (entityToUpdate == null && childNotFoundException != null)
            throw childNotFoundException;

        var oldModel = MapperObject.Map<TModel>(entityToUpdate);

        var updateModel = await getUpdatedModelTask(oldModel);
        MapperObject.Map(updateModel, entityToUpdate);

        // Сохраняем добавленный элемент (тут же он получает Id и т.п.)
        await Context.SaveChangesAsync();

        // Возвращаем DTO со старой и новой моделью
        return new UpdatedModel<TModel>(MapperObject.Map<TModel>(entityToUpdate), oldModel);
    }


    /// <summary>
    ///     Возвращает дочернюю коллекцию
    /// </summary>
    /// <typeparam name="TModel">Тип модели элемента для создания</typeparam>
    /// <typeparam name="TEntity">Тип сущности элемента для создания</typeparam>
    /// <typeparam name="TRootEntity">Тип сущности корневого элемента (который в DbSet)</typeparam>
    /// <typeparam name="TRootNotFoundException">Тип исключения, которое будет выброшего, если родительская сущность не найдена</typeparam>
    /// <param name="rootId">Id родительской сущности, свойством которой является пополняемая коллекция</param>
    /// <param name="dummy">Используйте метод Dummy, чтобы указать тип модели возвращяемой коллекции</param>
    /// <param name="dbSetAccessor">Метод для получения DbSet'a из контекста</param>
    /// <param name="collectionAccessor">Метод для получения коллекции из родительской сущности</param>
    /// <param name="exception">Исключение, которое следует выбросить, если родительская сущность не найдена в DbSet'е</param>
    /// <returns>Список моделей дочернеЙ коллекции</returns>
    protected async Task<List<TModel>> GetChildCollection<TModel, TEntity, TRootEntity, TRootNotFoundException>(
        long rootId,
        Func<TModel> dummy,
        Func<TContext, IQueryable<TRootEntity>> dbSetAccessor,
        Expression<Func<TRootEntity, List<TEntity>>> collectionAccessor,
        TRootNotFoundException exception
    )
        where TRootNotFoundException : Exception
        where TRootEntity : class, IHasId
        where TEntity : IHasId
    {
        // Находим родительский элемент, включив в него дочернюю коллекцию с помощью Include
        var rootEntity = await dbSetAccessor(Context).AsNoTracking().Include(collectionAccessor).OrderBy(e => e.Id)
            .FirstOrDefaultAsync(e => e.Id == rootId);
        if (rootEntity == null)
            throw exception; // Если не нашли, выбрасываем исключение

        // Возвращаем список моделей дочерней коллекции
        return MapperObject.Map<List<TModel>>(collectionAccessor.Compile().Invoke(rootEntity));
    }

    /// <summary>
    ///     Syncronizes the entity list.
    /// </summary>
    /// <remarks>
    ///     Проивзодит синхронизацию выбранных сущностей со списком переданных моделей: не существующие будут добавлены, лишние
    ///     удалены, измененные - изменены.
    /// </remarks>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="sourceModels">The source models.</param>
    /// <param name="dbSetQueryAccessor">The database set accessor to query existing entities</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entitiesToSyncronizeSelector">The entities to syncronize selector.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    /// <param name="createEntityFunc">Create new entity function. AutoMapper mapping by default</param>
    /// <param name="updateEntityFunc">Update existing entity function. AutoMapper mapping by default</param>
    /// <param name="retryOnUpdateException">
    ///     Performs one-time retry when db update exception received, in case of concurrency
    ///     issues
    /// </param>
    /// <returns>Syncronization result</returns>
    protected async Task<SyncronizationResultModel<TModel>> SyncronizeEntities<TModel, TEntity>(
        List<TModel> sourceModels,
        Func<TContext, IQueryable<TEntity>> dbSetQueryAccessor,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Expression<Func<TEntity, bool>> entitiesToSyncronizeSelector,
        Func<TModel, TEntity, bool> equalityComparer,
        Func<TModel, TEntity> createEntityFunc = null,
        Func<TModel, TEntity, TEntity> updateEntityFunc = null,
        bool retryOnUpdateException = false)
        where TEntity : class
    {
        Func<TModel, TEntity, TEntity> defaultUpdateFunc = (model, entity) => MapperObject.Map(model, entity);
        Func<TModel, TEntity> defaultCreateFunc = model => MapperObject.Map<TEntity>(model);
        updateEntityFunc = updateEntityFunc ?? defaultUpdateFunc;
        createEntityFunc = createEntityFunc ?? defaultCreateFunc;

        try
        {
            var newItems = new List<TEntity>();
            var updatedItems = new List<UpdatedModel<TModel>>();
            var existingEntities = await dbSetQueryAccessor(Context).Where(entitiesToSyncronizeSelector).ToListAsync();
            foreach (var sourceModel in sourceModels)
            {
                var existingEntity = existingEntities.FirstOrDefault(s => equalityComparer(sourceModel, s));
                if (existingEntity != null)
                {
                    var oldModel = MapperObject.Map<TModel>(existingEntity);
                    existingEntity = updateEntityFunc(sourceModel, existingEntity);
                    var updatedModel = MapperObject.Map<TModel>(existingEntity);
                    updatedItems.Add(new UpdatedModel<TModel>(updatedModel, oldModel));
                }
                else
                {
                    var newEntity = createEntityFunc(sourceModel);
                    dbSetAccessor(Context).Add(newEntity);
                    newItems.Add(newEntity);
                }
            }

            var deletedEntities = existingEntities.Where(e => sourceModels.All(source => !equalityComparer(source, e)));
            var deletedModels = MapperObject.Map<List<TModel>>(deletedEntities);
            dbSetAccessor(Context).RemoveRange(deletedEntities);

            await Context.SaveChangesAsync();

            return new SyncronizationResultModel<TModel>
            {
                NewItems = MapperObject.Map<List<TModel>>(newItems),
                UpdatedItems = updatedItems,
                DeletedItems = deletedModels
            };
        }
        catch (Exception ex) when (retryOnUpdateException &&
                                   (ex is DbUpdateException || ex is InvalidOperationException))
        {
            Logger.LogWarning($"Retrying after getting update exception: {ex.Message} : {ex}");

            Logger.LogDebug("Removing failed entity from db context");
            Context.Reset();

            return await SyncronizeEntities(sourceModels, dbSetQueryAccessor, dbSetAccessor,
                entitiesToSyncronizeSelector, equalityComparer, createEntityFunc, updateEntityFunc);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex is InvalidOperationException)
        {
            Logger.LogWarning(
                $"Failed to perform SerializeEntities operation for entities {sourceModels.Select(m => m.ToJsonFromComplexObject()).JoinToString(", ")}: {ex}");
            throw;
        }
    }

    /// <summary>
    ///     Syncronizes the entity list.
    /// </summary>
    /// <remarks>
    ///     Проивзодит синхронизацию выбранных сущностей со списком переданных моделей: не существующие будут добавлены, лишние
    ///     удалены, измененные - изменены.
    /// </remarks>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="sourceModels">The source models.</param>
    /// <param name="dbSetAccessor">The database set accessor.</param>
    /// <param name="entitiesToSyncronizeSelector">The entities to syncronize selector.</param>
    /// <param name="equalityComparer">The equality comparer.</param>
    /// <param name="updateEntityFunc">Update existing entity function. AutoMapper mapping by default</param>
    /// <param name="retryOnUpdateException">
    ///     Performs one-time retry when db update exception received, in case of concurrency
    ///     issues
    /// </param>
    /// <returns>Syncronization result</returns>
    protected Task<SyncronizationResultModel<TModel>> SyncronizeEntities<TModel, TEntity>(
        List<TModel> sourceModels,
        Func<TContext, DbSet<TEntity>> dbSetAccessor,
        Expression<Func<TEntity, bool>> entitiesToSyncronizeSelector,
        Func<TModel, TEntity, bool> equalityComparer,
        Func<TModel, TEntity, TEntity> updateEntityFunc = null,
        bool retryOnUpdateException = false)
        where TEntity : class
    {
        return SyncronizeEntities(sourceModels, dbSetAccessor, dbSetAccessor, entitiesToSyncronizeSelector,
            equalityComparer, null, updateEntityFunc, retryOnUpdateException);
    }

    /// <summary>
    ///     Пустой метод, используемый для передачи в метод, где требуется указать тип и нужно определение типа времени
    ///     компиляции
    /// </summary>
    /// <typeparam name="TAny">Произвольный тип, который хотим указать</typeparam>
    /// <returns>Не важно, что</returns>
    protected TAny Dummy<TAny>()
    {
        return default;
    }

    /// <summary>
    ///     Serializes object the without errors.
    /// </summary>
    protected string SerializeWithoutErrors(object obj)
    {
        return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        });
    }
}
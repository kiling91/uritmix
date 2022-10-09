using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Helpers.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Helpers.DataAccess.Relational.Extensions;

/// <summary>
///     Querable extensions
/// </summary>
public static class QuerableExtensions
{
    /// <summary>
    ///     Applies OrderBy or ThenBy with asc/desc ordering
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TColumn">The type of the column to order by.</typeparam>
    /// <param name="queryable">The queryable.</param>
    /// <param name="property">Property to be ordered by</param>
    /// <param name="order">IF true, ascending, if false - descending</param>
    /// <returns>Ordered queryable</returns>
    public static IQueryable<TEntity> AppendOrdering<TEntity, TColumn>(this IQueryable<TEntity> queryable,
        Expression<Func<TEntity, TColumn>> property, bool order)
    {
        if (!queryable.IsOrdered())
            return order ? queryable.OrderBy(property) : queryable.OrderByDescending(property);

        var orderedQueryable = queryable as IOrderedQueryable<TEntity>;
        return order ? orderedQueryable.ThenBy(property) : orderedQueryable.ThenByDescending(property);
    }

    /// <summary>
    ///     Creates the paginated list fron query
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageNumber">Number of the page.</param>
    /// <returns></returns>
    public static async Task<PaginatedList<TItem>> ToPaginatedListWithoutOrderingAsync<TItem>(
        this IQueryable<TItem> source, int pageSize, int pageNumber)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<TItem>(items, count, pageSize, pageNumber);
    }

    /// <summary>
    ///     Creates the paginated list fron query
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="source">The source query.</param>
    /// <returns></returns>
    public static Task<PaginatedList<TItem>> ToPaginatedListWithoutOrderingAsync<TItem>(this IQueryable<TItem> source,
        Paginator paginator)
    {
        return source.ToPaginatedListWithoutOrderingAsync(paginator.PageSize, paginator.PageNumber);
    }

    /// <summary>
    ///     Creates the paginated list fron query
    /// </summary>
    /// <typeparam name="TItemWithId">
    ///     The type of the item.<</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageNumber">Number of the page.</param>
    /// <returns></returns>
    public static async Task<PaginatedList<TItemWithId>> ToPaginatedListAsync<TItemWithId>(
        this IQueryable<TItemWithId> source, int pageSize, int pageNumber)
        where TItemWithId : IHasId
    {
        source.AppendOrdering(e => e.Id, true);

        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
        return new PaginatedList<TItemWithId>(items, count, pageSize, pageNumber);
    }

    /// <summary>
    ///     Creates the paginated list fron query
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="source">The source query.</param>
    /// <param name="paginator">The paginator.</param>
    /// <returns></returns>
    public static Task<PaginatedList<TItem>> ToPaginatedListAsync<TItem>(this IQueryable<TItem> source,
        Paginator paginator)
        where TItem : IHasId
    {
        return source.ToPaginatedListAsync(paginator.PageSize, paginator.PageNumber);
    }

    /// <summary>
    ///     Checks if collection is already ordeded
    /// </summary>
    public static bool IsOrdered<TEntity>(this IQueryable<TEntity> queryable)
    {
        if (!(queryable is IOrderedQueryable<TEntity>))
            return false;

        try
        {
            var orderedQueryable = queryable as IOrderedQueryable<TEntity>;
            orderedQueryable = orderedQueryable.ThenBy(e => 0);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    /// <summary>
    ///     OrderBy with column name
    /// </summary>
    /// <remarks>
    ///     Taken from here: https://stackoverflow.com/a/31959568/3094849
    /// </remarks>
    /// <param name="query"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="TSource"></typeparam>
    /// <returns></returns>
    public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> query, string propertyName)
    {
        var entityType = typeof(TSource);

        //Create x=>x.PropName
        var propertyInfo = entityType.GetProperty(propertyName);
        var arg = Expression.Parameter(entityType, "x");
        var property = Expression.Property(arg, propertyName);
        var selector = Expression.Lambda(property, arg);

        //Get System.Linq.Queryable.OrderBy() method.
        var enumarableType = typeof(Queryable);
        var method = enumarableType.GetMethods()
            .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
            .Where(m =>
            {
                var parameters = m.GetParameters().ToList();
                //Put more restriction here to ensure selecting the right overload                
                return parameters.Count == 2; //overload that has 2 parameters
            }).Single();
        //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
        var genericMethod = method.MakeGenericMethod(entityType, propertyInfo.PropertyType);

        /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
          Note that we pass the selector as Expression to the method and we don't compile it.
          By doing so EF can extract "order by" columns and generate SQL for it.*/
        var newQuery = (IOrderedQueryable<TSource>)genericMethod
            .Invoke(genericMethod, new object[] { query, selector });
        return newQuery;
    }
}
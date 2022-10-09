using System.Collections.Generic;
using System.Linq;

namespace Helpers.Pagination.Extensions;

public static class PaginatedListExtensions
{
    /// <summary>
    ///     Converts list to single page of PaginatedList with specified pageSize and pageNumber
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageNumber">The page number.</param>
    /// <returns>Page of paginated list wit specified size</returns>
    public static PaginatedList<TItem> ToPaginatedList<TItem>(this IEnumerable<TItem> items, int pageSize,
        int pageNumber)
    {
        var list = items as IList<TItem> ?? items.ToList();
        return new PaginatedList<TItem>(list.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(), list.Count,
            pageSize, pageNumber);
    }

    /// <summary>
    ///     Converts list to single page of PaginatedList with specified pageSize and pageNumber
    /// </summary>
    /// <typeparam name="TItem">The type of the item.</typeparam>
    /// <param name="items">The items.</param>
    /// <param name="paginator">The paginator.</param>
    /// <returns>
    ///     Page of paginated list wit specified size
    /// </returns>
    public static PaginatedList<TItem> ToPaginatedList<TItem>(this IEnumerable<TItem> items, Paginator paginator)
    {
        var list = items as IList<TItem> ?? items.ToList();
        return new PaginatedList<TItem>(
            list.Skip((paginator.PageNumber - 1) * paginator.PageSize).Take(paginator.PageSize).ToList(), list.Count,
            paginator.PageSize, paginator.PageNumber);
    }

    /// <summary>
    ///     Used to
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="existingPaginatedList">The existing paginated list.</param>
    /// <returns></returns>
    public static PaginatedList<TDestinationItem> ToExistingPaginatedList<TSourceItem, TDestinationItem>(
        this IEnumerable<TDestinationItem> items, PaginatedList<TSourceItem> existingPaginatedList)
    {
        return new PaginatedList<TDestinationItem>(items.ToList(), existingPaginatedList.TotalRecords,
            existingPaginatedList.PageSize, existingPaginatedList.PageNumber);
    }
}
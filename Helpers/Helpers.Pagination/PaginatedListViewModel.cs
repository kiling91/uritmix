using System.Collections.Generic;

namespace Helpers.Pagination;

/// <summary>
///     Paginated list surrogate
/// </summary>
public class PaginatedListViewModel<T>
{
    /// <summary>
    ///     The page number this page represents.
    /// </summary>
    public int PageNumber { get; set; }

    /// <summary>
    ///     The size of this page.
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    ///     The total number of pages available.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     The total number of records available.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    ///     If true, next page contains any data
    /// </summary>
    public bool NextPageExists { get; set; }

    /// <summary>
    ///     The records this page represents.
    /// </summary>
    public List<T> Results { get; set; }
}
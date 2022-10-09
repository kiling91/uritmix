using System;
using System.Collections.Generic;
using System.Linq;
using Helpers.Pagination.Exceptions;

namespace Helpers.Pagination;

/// <summary>
///     Paginated list structure
/// </summary>
/// <typeparam name="T"></typeparam>
public class PaginatedList<T> : List<T>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedList{T}" /> class.
    /// </summary>
    [Obsolete("For serialization only", true)]
    public PaginatedList()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedList{T}" /> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="totalCount">The total count.</param>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageNumber">The page number.</param>
    public PaginatedList(List<T> items, int totalCount, int pageSize, int pageNumber)
    {
        if (pageNumber < 1)
            throw new PaginationException("PageNumber should be >= 1, but got " + pageNumber);
        if (pageSize < 1)
            throw new PaginationException("PageSize should be >= 1, but got " + pageSize);

        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalCount;

        AddRange(items);
    }

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
    public int TotalPages => (int)Math.Ceiling(TotalRecords / (double)PageSize);

    /// <summary>
    ///     The total number of records available.
    /// </summary>
    public int TotalRecords { get; set; }

    /// <summary>
    ///     If true, next page contains any data
    /// </summary>
    public bool NextPageExists => PageNumber < TotalPages;

    /// <summary>
    ///     The records this page represents.
    /// </summary>
    public List<T> Results
    {
        get => this.ToList();
        set
        {
            Clear();
            AddRange(value);
        }
    }

    /// <summary>
    ///     Creates paginated lsit from empty collection
    /// </summary>
    public static PaginatedList<T> Empty(int pageSize, int pageNumber)
    {
        return new PaginatedList<T>(new List<T>(), 0, pageSize, pageNumber);
    }
}
using System.ComponentModel.DataAnnotations;

namespace Helpers.Pagination;

/// <summary>
///     Paginator
/// </summary>
public class Paginator
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Paginator" /> class.
    /// </summary>
    public Paginator()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Paginator" /> class.
    /// </summary>
    /// <param name="pageSize">Size of the page.</param>
    /// <param name="pageNumber">The page number.</param>
    public Paginator(int pageSize, int pageNumber)
    {
        PageSize = pageSize;
        PageNumber = pageNumber;
    }

    /// <summary>
    ///     Size of the page.
    /// </summary>
    [Required]
    public int PageSize { get; set; }

    /// <summary>
    ///     Page number
    /// </summary>
    [Required]
    public int PageNumber { get; set; }

    /// <summary>
    ///     Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>
    ///     A <see cref="System.String" /> that represents this instance.
    /// </returns>
    public override string ToString()
    {
        return $"pageSize={PageSize}&pageNumber={PageNumber}";
    }
}
using System;

namespace Helpers.Pagination.Exceptions;

/// <summary>
///     Pagination exception
/// </summary>
/// <seealso cref="System.Exception" />
public class PaginationException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginationException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public PaginationException(string message) :
        base(message)
    {
    }
}
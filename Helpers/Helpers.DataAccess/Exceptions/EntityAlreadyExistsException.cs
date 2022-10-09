using System;

namespace Helpers.DataAccess.Exceptions;

/// <summary>
///     Entity already exists exception
/// </summary>
/// <seealso cref="System.Exception" />
public abstract class EntityAlreadyExistsException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected EntityAlreadyExistsException(string message)
        : base(message)
    {
    }
}

public class EntityAlreadyExistsException<TEntity> : EntityAlreadyExistsException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException{TEntity}" /> class.
    /// </summary>
    public EntityAlreadyExistsException()
        : base($"Entity of type {EntityTypeName} with provided key already exists")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException{TEntity}" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected EntityAlreadyExistsException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Gets the name of the entity type.
    /// </summary>
    protected static string EntityTypeName => typeof(TEntity).Name.Replace("ViewModel", "").Replace("Model", "");
}
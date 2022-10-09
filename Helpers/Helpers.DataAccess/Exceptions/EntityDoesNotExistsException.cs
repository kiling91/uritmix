using System;

namespace Helpers.DataAccess.Exceptions;

/// <summary>
///     Entity does not exists exception
/// </summary>
/// <seealso cref="System.Exception" />
public abstract class EntityDoesNotExistsException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected EntityDoesNotExistsException(string message)
        : base(message)
    {
    }
}

/// <summary>
///     Entity does not exists exception
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <seealso cref="System.Exception" />
public class EntityDoesNotExistsException<TEntity> : EntityDoesNotExistsException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException{TEntity}" /> class.
    /// </summary>
    public EntityDoesNotExistsException()
        : base($"Entity of type {EntityTypeName} with provided key does not exists")
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityDoesNotExistsException{TEntity}" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected EntityDoesNotExistsException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Gets the name of the entity type.
    /// </summary>
    protected static string EntityTypeName => typeof(TEntity).Name.Replace("ViewModel", "").Replace("Model", "");
}
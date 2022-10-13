using System;

namespace Helpers.DataAccess.Exceptions;

public class EntityDoesNotExistsException<TEntity> : Exception
{
    public EntityDoesNotExistsException() : base($"Entity of type {EntityTypeName} with provided key does not exists")
    {
    }

    protected EntityDoesNotExistsException(string message) : base(message)
    {
    }

    private static string EntityTypeName => typeof(TEntity).Name.Replace("ViewModel", "").Replace("Model", "");
}
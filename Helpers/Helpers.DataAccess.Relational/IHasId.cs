namespace Helpers.DataAccess.Relational;

/// <summary>
///     Interface for object with Id property
/// </summary>
public interface IHasId
{
    /// <summary>
    ///     Id of the entity
    /// </summary>
    long Id { get; set; }
}
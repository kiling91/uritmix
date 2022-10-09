using AutoMapper;
using Helpers.Mapping.Extensions;
using Newtonsoft.Json;

namespace Helpers.Mapping;

/// <summary>
///     Custom mapping profile
/// </summary>
/// <seealso cref="AutoMapper.Profile" />
public class CustomProfile : Profile
{
    /// <summary>
    ///     Determines whether the specified object has all properties default.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     <c>true</c> if the specified object has default properties; otherwise, <c>false</c>.
    /// </returns>
    protected bool IsDefault<T>(T obj)
    {
        return obj.IsDefault();
    }

    /// <summary>
    ///     Serializes the list of strings to string, concatenating strings with ','
    /// </summary>
    /// <typeparam name="T">Type of the serializing object</typeparam>
    /// <param name="list">The list.</param>
    /// <returns>
    ///     Serialized list of strings
    /// </returns>
    [Obsolete("Use 'StoreAsJson' and 'LoadFromJson' methods instead")]
    protected string? SerializeList<T>(IEnumerable<T> list)
    {
        var stringsList = list?.Select(obj => obj?.ToString()).ToList();
        return stringsList != null ? JsonConvert.SerializeObject(stringsList) : null;
    }

    /// <summary>
    ///     Parses the serialized list of strings.
    /// </summary>
    /// <param name="serializedList">The serialized list.</param>
    /// <returns>List of strings</returns>
    [Obsolete("Use 'StoreAsJson' and 'LoadFromJson' methods instead")]
    protected List<string>? ParseListOfStrings(string serializedList)
    {
        return !string.IsNullOrWhiteSpace(serializedList)
            ? JsonConvert.DeserializeObject<List<string>>(serializedList)
            : null;
    }

    /// <summary>
    ///     Stores object as json.
    /// </summary>
    protected string? StoreAsJson<TObject>(TObject obj)
    {
        return obj != null ? JsonConvert.SerializeObject(obj) : null;
    }

    /// <summary>
    ///     Loads object from json.
    /// </summary>
    public TObject? LoadFromJson<TObject>(string objectJson) where TObject : class
    {
        return !string.IsNullOrWhiteSpace(objectJson) ? JsonConvert.DeserializeObject<TObject>(objectJson) : null;
    }
}
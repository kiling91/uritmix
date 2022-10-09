using Newtonsoft.Json;

namespace Helpers.Mapping.Extensions;

/// <summary>
///     Object extensions
/// </summary>
public static class ObjectExtensions
{
    /// <summary>
    ///     Determines whether the specified object has all properties default using Json serialization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj">The object.</param>
    /// <returns>
    ///     <c>true</c> if the specified object has default properties; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsDefault<T>(this T obj)
    {
        return JsonConvert.SerializeObject(obj) == JsonConvert.SerializeObject(Activator.CreateInstance<T>());
    }
}
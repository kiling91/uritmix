using Newtonsoft.Json;

namespace Helpers.Core.Extensions;

public static class ObjectExtensions
{
    /// <summary>
    ///     Returns list that contains object as a single item
    /// </summary>
    public static List<T> AsList<T>(this T obj)
    {
        return new List<T>
        {
            obj
        };
    }

    /// <summary>
    ///     Represents object as a result of completed task
    /// </summary>
    public static Task<T> AsTaskResult<T>(this T obj)
    {
        return Task.FromResult(obj);
    }

    public static string ToJsonFromComplexObject<T>(this T obj, Formatting formatting = Formatting.None,
        TypeNameHandling typeNameHandling = TypeNameHandling.All)
    {
        var settings = new JsonSerializerSettings
        {
            Error = (sender, args) => { args.ErrorContext.Handled = true; },
            TypeNameHandling = typeNameHandling,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = formatting
        };

        return JsonConvert.SerializeObject(obj, settings);
    }
}
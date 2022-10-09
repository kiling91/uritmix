using Helpers.Pagination.Converters;
using Newtonsoft.Json;

namespace Helpers.Pagination.Helpers;

/// <summary>
///     JsonConverterRegister
/// </summary>
public static class JsonConverterRegister
{
    /// <summary>
    ///     Performs global registration of <see cref="PaginatedListJsonConverter" />
    /// </summary>
    public static void RegisterPaginatedListJsonConverter()
    {
        var defaultJsonSettings = JsonConvert.DefaultSettings?.Invoke() ?? new JsonSerializerSettings();
        defaultJsonSettings.Converters.Add(new PaginatedListJsonConverter());
        JsonConvert.DefaultSettings = () => defaultJsonSettings;
    }
}
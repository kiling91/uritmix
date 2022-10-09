using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Helpers.Pagination.Converters;

public class PaginatedListJsonConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return IsSubclassOfRawGeneric(typeof(PaginatedList<>), objectType);
    }

    public override object ReadJson(
        JsonReader reader, Type objectType,
        object existingValue, JsonSerializer serializer)
    {
        var collectionItemType = objectType.GetGenericArguments().First();
        var surrogateType = typeof(PaginatedListViewModel<>).MakeGenericType(collectionItemType);

        var paginatedList = Activator.CreateInstance(objectType);

        var surrogate = serializer.Deserialize(reader, surrogateType);
        foreach (var prop in surrogateType.GetProperties())
        {
            var objectProperty = objectType.GetProperty(prop.Name);
            if (objectProperty.SetMethod != null) objectProperty.SetValue(paginatedList, prop.GetValue(surrogate));
        }

        return paginatedList;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var itemType = value.GetType().GetGenericArguments().First();
        var surrogateType = typeof(PaginatedListViewModel<>).MakeGenericType(itemType);

        var surrogate = Activator.CreateInstance(surrogateType);
        foreach (var prop in surrogateType.GetProperties())
        {
            var propValue = value.GetType().GetProperty(prop.Name).GetValue(value);
            surrogateType.GetProperty(prop.Name).SetValue(surrogate, propValue);
        }

        serializer.Serialize(writer, surrogate);
    }

    private bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.GetTypeInfo().IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur) return true;
            toCheck = toCheck.GetTypeInfo().BaseType;
        }

        return false;
    }
}
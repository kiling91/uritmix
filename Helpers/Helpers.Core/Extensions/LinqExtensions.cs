namespace Helpers.Core.Extensions;

public static class LinqExtensions
{
    /// <summary>
    ///     Splits collection to collection of groups of provided size
    /// </summary>
    public static IEnumerable<IEnumerable<T>> SplitToGroups<T>(this IEnumerable<T> collection, int groupMaxSize)
    {
        return collection.Select((item, index) => new { Item = item, Index = index })
            .GroupBy(i => i.Index / groupMaxSize).Select(g => g.Select(e => e.Item));
    }

    /// <summary>
    ///     Splits collection to collection of groups of provided groups count
    /// </summary>
    public static IEnumerable<IEnumerable<T>> SplitToGroupsCount<T>(this IEnumerable<T> collection, int groupsCount)
    {
        return collection.Select((item, index) => new { Item = item, Index = index })
            .GroupBy(i => i.Index % groupsCount).Select(g => g.Select(e => e.Item));
    }

    /// <summary>
    ///     Groups objects by key and selects first element in group
    /// </summary>
    public static IEnumerable<T> DistinctBy<T, TProperty>(this IEnumerable<T> collection,
        Func<T, TProperty> keySelector)
    {
        return collection.GroupBy(keySelector).Select(g => g.First()).ToList();
    }

    /// <summary>
    ///     Returns null, if collection is empty, otherwize returns same collection
    /// </summary>
    /// <param name="collection"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> NullIfEmpty<T>(this IEnumerable<T> collection)
    {
        var list = collection.ToList();
        return list.Any() ? list : null;
    }

    /// <summary>
    ///     Returns empty collection if collection is null
    /// </summary>
    public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> collection)
    {
        return collection ?? new List<T>();
    }

    /// <summary>
    ///     Shows if collection is null or empty
    /// </summary>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> collection)
    {
        return collection == null || !collection.Any();
    }

    /// <summary>
    ///     Shows if collection is empty
    /// </summary>
    public static bool IsEmpty<T>(this IEnumerable<T> collection)
    {
        return collection != null && !collection.Any();
    }

    /// <summary>
    ///     Joins provided collection elements to single string using provided separator
    /// </summary>
    public static string JoinToString<T>(this IEnumerable<T> collection, string separator)
    {
        if (collection == null)
            return string.Empty;

        return string.Join(separator, collection);
    }

    /// <summary>
    ///     Concatenates collections, if provided collection is not null
    /// </summary>
    public static IEnumerable<T> ConcatIfNotNull<T>(this IEnumerable<T> collection, IEnumerable<T> elems)
    {
        return elems != null ? collection.Concat(elems) : collection;
    }

    /// <summary>
    ///     Prepends collection with element, if it is not null
    /// </summary>
    public static IEnumerable<T> PrependIfNotNull<T>(this IEnumerable<T> collection, T elem) where T : class
    {
        return elem != null ? collection.Prepend(elem) : collection;
    }

    /// <summary>
    ///     Appends element to collection, if it is not null
    /// </summary>
    public static IEnumerable<T> AppendIfNotNull<T>(this IEnumerable<T> collection, T elem) where T : class
    {
        return elem != null ? collection.Append(elem) : collection;
    }

    /// <summary>
    ///     Applies specified action to each element of the collection
    /// </summary>
    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action) where T : class
    {
        return collection.Select(e =>
        {
            action(e);
            return e;
        }).ToList();
    }

    /// <summary>
    ///     Checks if item is contained in the collection
    /// </summary>
    public static bool In<T>(this T item, IEnumerable<T> collection)
    {
        return collection.Contains(item);
    }

    /// <summary>
    ///     Checks if item is contained in the collection
    /// </summary>
    public static bool In<T>(this T item, params T[] collection)
    {
        return item.In(collection.AsEnumerable());
    }

    /// <summary>
    ///     Checks that collection is a subset for other collection
    /// </summary>
    public static bool ContainsAll<T>(this IEnumerable<T> collection, IEnumerable<T> subset)
    {
        return subset.All(collection.Contains);
    }

    /// <summary>
    ///     Excludes items matching provided filter
    /// </summary>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, Predicate<T> filterToExclude)
    {
        return collection.Where(item => !filterToExclude(item));
    }

    /// <summary>
    ///     Excludes items matching provided filter
    /// </summary>
    public static IEnumerable<T> Except<T>(this IEnumerable<T> collection, T item)
    {
        return collection.Except(item.AsList());
    }

    /// <summary>
    ///     Selects values recurcively
    /// </summary>
    public static List<TResult> SelectRecurcive<TObject, TResult>(this TObject obj,
        Func<TObject, TResult> resultSelector, Func<TObject, TObject> recurcivePropertySelector, int maxDepth = -1)
    {
        var results = resultSelector(obj).AsList();

        if (maxDepth == 0)
            return results;

        var childProperty = recurcivePropertySelector(obj);
        if (childProperty != null)
        {
            var childResults = childProperty.SelectRecurcive(resultSelector, recurcivePropertySelector,
                maxDepth == -1 ? -1 : maxDepth - 1);
            results = results.Concat(childResults).ToList();
        }

        return results;
    }

    /// <summary>
    ///     Selects values recurcively
    /// </summary>
    public static List<TResult> SelectRecurcive<TObject, TResult>(this TObject obj,
        Func<TObject, TResult> resultSelector, Func<TObject, IEnumerable<TObject>> recurcivePropertySelector,
        int maxDepth = -1)
    {
        var results = resultSelector(obj).AsList();

        if (maxDepth == 0)
            return results;

        var childProperty = recurcivePropertySelector(obj);
        if (childProperty != null)
        {
            var childResults = childProperty.SelectMany(prop =>
                prop.SelectRecurcive(resultSelector, recurcivePropertySelector, maxDepth == -1 ? -1 : maxDepth - 1));
            results = results.Concat(childResults).ToList();
        }

        return results;
    }

    /// <summary>
    ///     Applies action to each nested element recurcivelly
    /// </summary>
    public static void ApplyRecurcive<TObject>(this TObject obj, Action<TObject> action,
        Func<TObject, TObject> recurcivePropertySelector, int maxDepth = -1)
    {
        action(obj);

        if (maxDepth == 0)
            return;

        var childProperty = recurcivePropertySelector(obj);
        if (childProperty == null)
            return;

        childProperty.ApplyRecurcive(action, recurcivePropertySelector, maxDepth == -1 ? -1 : maxDepth - 1);
    }

    /// <summary>
    ///     Applies action to each nested element recurcivelly
    /// </summary>
    public static void ApplyRecurcive<TObject>(this TObject obj, Action<TObject> action,
        Func<TObject, IEnumerable<TObject>> recurcivePropertySelector, int maxDepth = -1)
    {
        action(obj);

        if (maxDepth == 0)
            return;

        var childProperties = recurcivePropertySelector(obj);

        foreach (var childProperty in childProperties) childProperty.ApplyRecurcive(action, recurcivePropertySelector);
    }


    public static async Task<List<TResult>> SelectAsync<TSource, TResult>(this IEnumerable<TSource> sources,
        Func<TSource, Task<TResult>> taskFactory)
    {
        var results = new List<TResult>();

        foreach (var source in sources)
        {
            var result = await taskFactory(source);
            results.Add(result);
        }

        return results;
    }

    public static async Task<List<TResult>> SelectManyAsync<TSource, TCollection, TResult>(
        this IEnumerable<TSource> sources,
        Func<TSource, Task<List<TCollection>>> taskFactory,
        Func<TSource, TCollection, TResult> resultSelector)
    {
        var results = new List<TResult>();

        foreach (var source in sources)
        {
            var collection = await taskFactory(source);
            foreach (var elem in collection) results.Add(resultSelector(source, elem));
        }

        return results;
    }

    public static async Task<List<TResult>> SelectManyAsync<TSource, TResult>(this IEnumerable<TSource> sources,
        Func<TSource, Task<List<TResult>>> taskFactory)
    {
        var results = new List<TResult>();

        foreach (var source in sources)
        {
            var collection = await taskFactory(source);
            foreach (var elem in collection) results.Add(elem);
        }

        return results;
    }
}
using AutoMapper;

namespace Helpers.Mapping;

/// <summary>
///     Multi mapper
/// </summary>
public abstract class MultiMapperBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MultiMapperBase" /> class.
    /// </summary>
    /// <param name="mapperObject">The mapper.</param>
    protected MultiMapperBase(IMapper mapperObject)
    {
        MapperObject = mapperObject;
    }

    public IMapper MapperObject { get; }

    /// <summary>
    ///     Maps the specified sources to object of specified type
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="sources">The sources.</param>
    /// <returns></returns>
    protected TResult Map<TResult>(params object[] sources)
    {
        var result = Activator.CreateInstance<TResult>();
        foreach (var src in sources)
        {
            if (src == null)
                continue;

            MapperObject.Map(src, result);
        }

        return result;
    }

    /// <summary>
    ///     Maps the specified sources to existing target
    /// </summary>
    protected TResult Map<TResult>(TResult target, params object[] sources)
    {
        foreach (var src in sources)
        {
            if (src == null)
                continue;

            MapperObject.Map(src, target);
        }

        return target;
    }
}
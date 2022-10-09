using System.Linq.Expressions;
using AutoMapper;

namespace Helpers.Mapping;

/// <summary>
///     Mapping expression extensions
/// </summary>
public static class MappingExpressionExtensions
{
    /// <summary>
    ///     Ignores the Id property mappings
    /// </summary>
    /// <typeparam name="TSrc">The type to map from.</typeparam>
    /// <typeparam name="TDst">The type to map to</typeparam>
    /// <param name="mappingExpression">The mapping expression.</param>
    /// <param name="idPropertyName">Name of the Id property, defaults to Id</param>
    /// <returns>Mapping expression</returns>
    public static IMappingExpression<TSrc, TDst> IgnoreId<TSrc, TDst>(
        this IMappingExpression<TSrc, TDst> mappingExpression, string idPropertyName = "Id")
    {
        return mappingExpression.ForMember(idPropertyName, opt => opt.Ignore());
    }

    /// <summary>
    ///     Ignores the specified property.
    /// </summary>
    /// <typeparam name="TSrc">The type of the source.</typeparam>
    /// <typeparam name="TDst">The type of the DST.</typeparam>
    /// <typeparam name="TMember">The type of the member.</typeparam>
    /// <param name="mappingExpression">The mapping expression.</param>
    /// <param name="ignoredProperty">The ignored property.</param>
    /// <returns></returns>
    public static IMappingExpression<TSrc, TDst> Ignore<TSrc, TDst, TMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression, Expression<Func<TDst, TMember>> ignoredProperty)
    {
        return mappingExpression.ForMember(ignoredProperty, opt => opt.Ignore());
    }

    /// <summary>
    ///     Creates the reverse mapping
    /// </summary>
    /// <typeparam name="TSrc">The type of the source.</typeparam>
    /// <typeparam name="TDst">The type of the DST.</typeparam>
    /// <param name="mappingExpression">The mapping expression.</param>
    /// <param name="profile">The profile.</param>
    /// <returns></returns>
    public static IMappingExpression<TDst, TSrc> ReverseMapExtended<TSrc, TDst>(
        this IMappingExpression<TSrc, TDst> mappingExpression, Profile profile)
    {
        return profile.CreateMap<TDst, TSrc>();
    }

    /*
    /// <summary>
    /// Shortcut for MapFrom method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> Map<TSrc, TDst, TMember>(this IMappingExpression<TSrc, TDst> mappingExpression, 
                                                                          Expression<Func<TDst, TMember>> dstMember, Expression<Func<TSrc, TMember>> srcMember)
    {
        return mappingExpression.ForMember(dstMember, opt => opt.MapFrom(srcMember));
    }
    */
    /// <summary>
    ///     Shortcut for MapFrom method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> Map<TSrc, TDst, TSrcMember, TDescMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TDescMember>> dstMember, Expression<Func<TSrc, TSrcMember>> srcMember)
    {
        return mappingExpression.ForMember(dstMember, opt => opt.MapFrom(srcMember));
    }

    /// <summary>
    ///     Shortcut for MapFrom from the source object method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> MapFromSource<TSrc, TDst, TMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TMember>> dstMember)
    {
        return mappingExpression.ForMember(dstMember, opt => opt.MapFrom(m => m));
    }

    /// <summary>
    ///     Shortcut for MapFrom method with PreCondition
    /// </summary>
    public static IMappingExpression<TSrc, TDst> MapWhen<TSrc, TDst, TMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TMember>> dstMember, Expression<Func<TSrc, TMember>> srcMember,
        Func<TSrc, bool> srcMemberCondition)
    {
        return mappingExpression.ForMember(dstMember, opt =>
        {
            opt.PreCondition(srcMemberCondition);
            opt.MapFrom(srcMember);
        });
    }

    /// <summary>
    ///     Shortcut for ResolveUsing method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> Resolve<TSrc, TDst, TDstMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TDstMember>> dstMember, Func<TSrc, TDstMember> srcMember)
    {
        return mappingExpression.ForMember(dstMember, opt =>
        {
            opt.MapFrom((src, dst) =>
            {
                try
                {
                    return srcMember(src);
                }
                catch (NullReferenceException)
                {
                    return default;
                }
                catch (ArgumentNullException)
                {
                    return default;
                }
            });
        });
    }

    /// <summary>
    ///     Shortcut for ResolveUsing method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> Resolve<TSrc, TDst, TMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TMember>> dstMember,
        Func<TSrc, TDst, TMember> srcMember)
    {
        return mappingExpression.ForMember(dstMember, opt => opt.MapFrom((src, dst) =>
        {
            try
            {
                return srcMember(src, dst);
            }
            catch (NullReferenceException)
            {
                return default;
            }
            catch (ArgumentNullException)
            {
                return default;
            }
        }));
    }

    /// <summary>
    ///     Shortcut for ResolveUsing method
    /// </summary>
    public static IMappingExpression<TSrc, TDst> Resolve<TSrc, TDst, TMember>(
        this IMappingExpression<TSrc, TDst> mappingExpression,
        Expression<Func<TDst, TMember>> dstMember, TMember value)
    {
        return mappingExpression.ForMember(dstMember, opt => opt.MapFrom(src => value));
    }
}
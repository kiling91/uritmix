using System;
using System.Linq.Expressions;

namespace Helpers.DataAccess.Relational.Extensions;

/// <summary>
///     Predicate builder
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    ///     Bool expressin, returning true
    /// </summary>
    public static Expression<Func<T, bool>> True<T>()
    {
        return f => true;
    }

    /// <summary>
    ///     Bool expressin, returning false
    /// </summary>
    public static Expression<Func<T, bool>> False<T>()
    {
        return f => false;
    }

    /// <summary>
    ///     Joins bool expression with OR logical statement
    /// </summary>
    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

        return Expression.Lambda<Func<T, bool>>(Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
    }

    /// <summary>
    ///     Joins bool expression with AND logical statement
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var invokedExpr = Expression.Invoke(expr2, expr1.Parameters);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
    }
}
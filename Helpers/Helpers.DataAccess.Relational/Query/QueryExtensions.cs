using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using Helpers.Core;

namespace Helpers.DataAccess.Relational.Query;

public static class QueryExtensions
{
    public static IQueryable<T> Filter<T>(this IQueryable<T> query, string filterExpression) where T : class
    {
        if (string.IsNullOrWhiteSpace(filterExpression) || filterExpression == "null")
            return query;

        try
        {
            var ex = new ExpressionParser();
            var linq = ex.Filter(filterExpression);
            if (linq == null)
                throw new ArgumentNullException(nameof(linq));
            
            query = query.Where(linq);
        }
        catch (Exception)
        {
            throw new RestBadRequestException("Invalid filter expression format");
        }

        return query;
    }
}
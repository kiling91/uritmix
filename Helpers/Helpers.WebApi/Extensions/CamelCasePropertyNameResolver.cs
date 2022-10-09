using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using FluentValidation.Internal;

namespace Helpers.WebApi.Extensions;

public static class CamelCasePropertyNameResolver
{
    public static string ResolvePropertyName(Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
        return ToCamelCase(DefaultPropertyNameResolver(type, memberInfo, expression));
    }

    private static string DefaultPropertyNameResolver(Type type, MemberInfo memberInfo, LambdaExpression expression)
    {
        var chain = PropertyChain.FromExpression(expression);
        if (chain.Count > 0) return chain.ToString();
        return memberInfo.Name;
    }

    private static string ToCamelCase(string str)
    {
        var array = str.Split(".");
        var result = new List<string>();

        foreach (var s in array)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0])) return s;

            var chars = s.ToCharArray();

            for (var i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i])) break;

                var hasNext = i + 1 < chars.Length;
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1])) break;

                chars[i] = char.ToLower(chars[i], CultureInfo.InvariantCulture);
            }

            result.Add(new string(chars));
        }

        return string.Join(".", result.ToArray());
    }
}
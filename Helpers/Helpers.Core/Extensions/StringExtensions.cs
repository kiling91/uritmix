using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Helpers.Core.Extensions;

public static class StringExtensions
{
    private static bool IsValidSymbols(this char value)
    {
        return value is ' ' or '(' or ')' or '-' or '_' or '+' or '=';
    }

    public static string ReplaceSpecialCharacters(this string value)
    {
        var result = "";
        foreach (var c in value)
            if (char.IsLetterOrDigit(c) || c.IsValidSymbols())
                result += c;
        return Strings.Trim(result);
    }

    public static (bool, string?) IsPhone(string value)
    {
        value = value.Trim();
        var pattern = @"^\+\s*\d{1,3}\s*\(\s*[0-9]\d{2}\)\s*\d{3}\s*-\s*\d{4}$";
        var options = RegexOptions.Multiline;
        foreach (Match _ in Regex.Matches(value, pattern, options))
            return (true, value);
        return (false, null);
    }

    public static string FirstLetterToUpper(this string str)
    {
        str = str.Trim();
        if (str.Length > 1)
            return char.ToUpper(str[0]) + str.Substring(1);
        return str.ToUpper();
    }
}
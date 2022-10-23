using System.Text.RegularExpressions;
using Microsoft.VisualBasic;

namespace Helpers.Core.Extensions;

public static class DateExtensions
{
    public static long ToUnixTimestamp(this DateTime date)
    {
        return ((DateTimeOffset)date.ToUniversalTime()).ToUnixTimeSeconds();
    }
    
    public static DateTime FromUnixTimestamp(this long date)
    {
        var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        return dateTime.AddSeconds(date).ToUniversalTime();;
    }
}
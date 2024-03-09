using System.Globalization;

namespace trb_officer_backend.Common;

public static class Util
{
    public static DateTime FromMs(long milliSec)
    {
        return DateTime.UnixEpoch.AddMilliseconds(milliSec);
    }

    public static long ToMs(string dateString)
    {
        // Try parsing the date string in the specified format
        if (DateTime.TryParseExact(dateString, "yyyy-MM-dd", null, DateTimeStyles.None, out var date))
        {
            // Convert the DateTime object to epoch time in milliseconds
            return (long)date.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        // Handle invalid date format
        throw new ArgumentException("Invalid date format. Please provide a date in YYYY-MM-DD format.");
    }
}
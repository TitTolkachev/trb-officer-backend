namespace trb_officer_backend.Common;

public static class Util
{
    
    public static DateTime FromMs(long milliSec)
    {
        var startTime = new DateTime(1970, 1, 1);
        var time = TimeSpan.FromMilliseconds(milliSec);
        return startTime.Add(time);
    }
}
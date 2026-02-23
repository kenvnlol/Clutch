namespace Clutch.Features.Shared.Extensions;

public static class DateTimeExtensions
{
    public static DateTime GetPreviousAtMidnight(this DateTime from, DayOfWeek dayOfWeek)
    {
        int daysBack = ((int)from.DayOfWeek - (int)dayOfWeek + 7) % 7;
        return from.Date.AddDays(-daysBack);
    }

    public static DateTime GetNextAtMidnight(this DateTime from, DayOfWeek dayOfWeek)
    {
        int daysForward = ((int)dayOfWeek - (int)from.DayOfWeek + 7) % 7;
        return from.Date.AddDays(daysForward);
    }

    public static DateTime ToUtc(this DateTime localTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeToUtc(localTime, timeZone);
    }

    public static DateTime FromUtc(this DateTime utcTime, TimeZoneInfo timeZone)
    {
        return TimeZoneInfo.ConvertTimeFromUtc(utcTime, timeZone);
    }
}

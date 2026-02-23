namespace Clutch.Infrastructure.Hangfire;

public static class CronExpressions
{
    public const string EverySecond = "*/1 * * * * *";
    public const string EveryFiveSeconds = "*/5 * * * * *";
    public const string EveryMinute = "*/1 * * * *";
    public const string EveryFiveMinutes = "*/5 * * * *";
    public const string Hourly = "0 * * * *";
    public const string DailyAtMidnight = "0 0 * * *";
    public const string WeeklyOnSundayMidnight = "0 0 * * 0";
}
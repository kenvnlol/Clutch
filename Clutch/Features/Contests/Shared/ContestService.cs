namespace Clutch.Features.Contests.Shared;

public static class ContestService
{
    private static readonly TimeZoneInfo ContestTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");

    /// <summary>
    /// Gets the current contest datetime range. 
    /// A contest is currently defined as running between Monday 00:00 to Sunday 12:00.
    /// </summary>
    /// <returns></returns>
    public static (DateTime StartUtc, DateTime EndUtc) GetCurrentContestPeriod()
    {
        var nowUtc = DateTime.UtcNow;

        var nowLocal = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, ContestTimeZone);

        var thisMondayLocal = nowLocal.Date.AddDays(-((int)nowLocal.DayOfWeek + 6) % 7);
        var thisSundayNoonLocal = thisMondayLocal.AddDays(6).AddHours(12);

        var startUtc = TimeZoneInfo.ConvertTimeToUtc(thisMondayLocal, ContestTimeZone);
        var endUtc = TimeZoneInfo.ConvertTimeToUtc(thisSundayNoonLocal, ContestTimeZone);

        if (nowUtc >= endUtc)
        {
            thisMondayLocal = thisMondayLocal.AddDays(7);
            thisSundayNoonLocal = thisSundayNoonLocal.AddDays(7);

            startUtc = TimeZoneInfo.ConvertTimeToUtc(thisMondayLocal, ContestTimeZone);
            endUtc = TimeZoneInfo.ConvertTimeToUtc(thisSundayNoonLocal, ContestTimeZone);
        }

        return (startUtc, endUtc);
    }

    public static string GetWinnerMessage(int placement, decimal prizeAmount)
    {
        return
            $"Congrats! You've placed {GetFormattedPlacement(placement)} in this week's Clutch Contest and won ${prizeAmount:C0}!\n\n" +
            $"Your prize has been added to your wallet. 🏆 You can view your updated balance under **User Settings → Wallet**.\n\n" +
            $"You can withdraw your winnings anytime using PayPal or crypto by selecting a withdrawal method from the same page.\n\n" +
            $"Hope to see your next fire clip in the contest next week!";
    }

    private static string GetFormattedPlacement(int placement)
        => placement switch
        {
            1 => "1st",
            2 => "2nd",
            3 => "3rd",
            _ => $"{placement}th"
        };


    public static decimal GetPrizeAmountForPlacement(int placement)
        => placement switch
        {
            1 => 500,
            2 => 200,
            3 => 100,
            4 => 50,
            5 => 50,
            _ => 0m
        };
}

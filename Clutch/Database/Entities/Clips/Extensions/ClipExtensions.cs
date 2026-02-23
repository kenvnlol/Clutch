using Clutch.Features.Contests.Shared;

namespace Clutch.Database.Entities.Clips.Extensions;

public static class ClipExtensions
{
    public static IQueryable<Clip> GetInCurrentContest(this IQueryable<Clip> queryable)
    {
        var (startUtc, endUtc) = ContestService.GetCurrentContestPeriod();
        return queryable.Where(c => c.Timestamp >= startUtc && c.Timestamp < endUtc);
    }
}

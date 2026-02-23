namespace Clutch.Features.Shared.Extensions;

public static class StatsAggregationExtensions
{
    /// <summary>
    /// Groups items by a specified key, selects the latest item per group by timestamp,
    /// and returns the sum of deltas (+1 for active, -1 for deleted).
    /// </summary>
    public static int SumDeltaByLatest<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> groupBySelector,
        Func<T, DateTimeOffset> timestampSelector,
        Func<T, bool> isDeletedSelector)
    {
        return source
            .GroupBy(groupBySelector)
            .Select(g => g.OrderByDescending(timestampSelector).First())
            .Sum(item => isDeletedSelector(item) ? -1 : 1);
    }
}
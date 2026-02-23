namespace Clutch.Features.Shared.Extensions;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Calculates the net delta (+1 or -1) for binary interactions,
    /// using the latest event per (AuthorId, ClipId) group.
    /// </summary>
    public static int GetDeltaBinary<T>(
        this IEnumerable<T>? source,
        Func<T, object> groupBySelector,
        Func<T, DateTimeOffset> timestampSelector,
        Func<T, bool> isDeletedSelector)
            => source?
                .GroupBy(groupBySelector)
                .Select(g => g.OrderByDescending(timestampSelector).First())
                .Sum(item => isDeletedSelector(item) ? -1 : 1) ?? 0;


    /// <summary>
    /// Calculates the net delta (+1 or -1) by summing all interactions,
    /// without grouping or filtering — for stackable events like comments.
    /// </summary>
    public static int GetDeltaCumulative<T>(
        this IEnumerable<T>? source,
        Func<T, bool> isDeletedSelector)
            => source?.Sum(item => isDeletedSelector(item) ? -1 : 1) ?? 0;
}

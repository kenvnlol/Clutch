//using Clutch.API.Database.Entities.DenormalizedCounterQueues;

//namespace Clutch.API.Features.Shared.Extensions;

//public static class StatsQueueBaseExtensions
//{
//    /// <summary>
//    /// Groups the queue by EntityType and collects distinct EntityIds into a dictionary.
//    /// </summary>
//    public static Dictionary<string, HashSet<int>> ToEntityIdLookup(this IEnumerable<StatsQueueBase> queue)
//    {
//        return queue
//            .GroupBy(q => q.EntityType)
//            .ToDictionary(
//                g => g.Key,
//                g => g.Select(q => q.EntityId).ToHashSet());
//    }
//}

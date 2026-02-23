using Clutch.Database.Entities.UserEvents;

namespace Clutch.Database.Entities;

public static class EventConsumerOffsetExtensions
{
    public static void AdvanceTo<T>(
        this EventConsumerOffset offset,
        List<CompactionResult<T>> compacted)
        where T : class
    {
        var lastEventId = compacted.LastOrDefault()?.EventId;
        if (lastEventId.HasValue)
        {
            offset.LastProcessedEventId = lastEventId.Value;
        }
    }
}

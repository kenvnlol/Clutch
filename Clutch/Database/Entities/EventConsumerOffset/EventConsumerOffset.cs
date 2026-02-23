namespace Clutch.Database.Entities;

/// <summary>
/// This has a dedicated table to avoid doing updates to UserEvent table to avoid deadlocks.
/// Pre-seeded.
/// </summary>
public class EventConsumerOffset
{
    public int Id { get; set; }
    public required EventConsumerGroup ConsumerGroup { get; init; }
    public int LastProcessedEventId { get; set; }
}

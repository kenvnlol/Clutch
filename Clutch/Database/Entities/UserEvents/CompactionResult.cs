using Clutch.Database.Entities.UserEvents;

public sealed record CompactionResult<T>(
    int EventId,
    T Data,
    int TotalCount,
    EventActionType Type)
    where T : class;



using Clutch.Database.Entities.DirectThreads;
using Clutch.Database.Entities.Users;
using Clutch.Database.Interceptors;

namespace Clutch.Database.Entities.DirectMessages;

public class DirectMessage : ISoftDeletable
{
    public int Id { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public DateTimeOffset? ReadAt { get; set; }
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public required string Text { get; init; }
    public int DirectThreadId { get; init; }
    public DirectThread DirectThread { get; init; } = null!;
    public bool IsDeleted { get; set; }

    public required int EventId { get; init; }
}
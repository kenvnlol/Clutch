using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.Mentions;

public class Mention
{
    public int Id { get; set; }
    public required string InitiatorUserId { get; init; }
    public User InitiatorUser { get; init; } = null!;
    public required string TargetUserId { get; init; }
    public User TargetUser { get; init; } = null!;
    public required DateTimeOffset Timestamp { get; init; }
    public required MentionSource Source { get; init; }
    public required InboxActivity InboxActivity { get; init; }
    public bool IsDeleted { get; set; }
    public required int EventId { get; init; }
}
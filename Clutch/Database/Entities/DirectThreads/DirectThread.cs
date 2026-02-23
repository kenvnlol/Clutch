using Clutch.Database.Entities.DirectMessages;
using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.DirectThreads;

public class DirectThread
{
    public int Id { get; init; }
    public required string ParticipantAId { get; init; }
    public User ParticipantA { get; init; } = null!;
    public required string ParticipantBId { get; init; }
    public User ParticipantB { get; init; } = null!;
    public DateTimeOffset? LastMessageAt { get; set; }
    public string? LastMessage { get; set; }
    public bool HasNewUnreadMessage { get; set; }
    public DateTimeOffset? ParticipantALastReadAt { get; set; }
    public DateTimeOffset? ParticipantBLastReadAt { get; set; }
    public required List<DirectMessage> Messages { get; init; }
}

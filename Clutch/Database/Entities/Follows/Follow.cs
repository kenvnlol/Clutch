using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.Follows;

public class Follow : ICompactionKeyProvider, IInboxEvent
{
    public int Id { get; set; }
    public required string InitiatorUserId { get; init; }
    public User InitiatorUser { get; init; } = null!;
    public required string TargetUserId { get; init; }
    public User TargetUser { get; init; } = null!;
    public required DateTimeOffset Timestamp { get; init; }
    public required InboxActivity InboxActivity { get; init; }
    public bool IsDeleted { get; set; }

    public required int EventId { get; init; }

    /// <summary>
    /// In-memory toggle state for cache/materializer:
    /// true = user followed; false = user unfollowed (tombstone).
    /// Not persisted to the database.
    /// </summary>
    [NotMapped]
    public FollowType FollowType { get; init; }
}
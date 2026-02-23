using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.CommentLikes;

public class CommentLike : ICompactionKeyProvider, IInboxEvent
{
    public int Id { get; set; }
    public required string InitiatorUserId { get; init; }
    public User InitiatorUser { get; init; } = null!;
    public required DateTimeOffset Timestamp { get; init; }
    public required InboxActivity InboxActivity { get; init; }
    public long CommentId { get; init; }
    public Comment Comment { get; init; } = null!;
    public bool IsDeleted { get; set; }

    /// <summary>
    /// In-memory toggle state for cache/materializer:
    /// true = user liked; false = user unliked (tombstone).
    /// Not persisted to the database.
    /// </summary>
    [NotMapped]
    public required LikeType LikeType { get; set; }
    public required int EventId { get; init; }
}



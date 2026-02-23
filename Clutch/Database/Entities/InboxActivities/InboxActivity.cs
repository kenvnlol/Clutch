using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.Mentions;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Users;
using Clutch.Database.Interceptors;

namespace Clutch.Database.Entities.InboxActivities;

/// <summary>
/// The point of this entity is to easily paginate just one entity for inbox activities and not having to call the db multiple times to construct the page. 
/// </summary>
public class InboxActivity : ISoftDeletable
{
    public int Id { get; set; }
    public DateTimeOffset? ReadAt { get; set; }
    public required InboxNotificationType Type { get; init; }
    public required DateTimeOffset Timestamp { get; set; }
    public string? DestinationUri { get; set; }
    public required string InitiatorId { get; init; }
    public User Initiator { get; init; } = null!;
    public required string UserInboxId { get; init; }
    public UserInbox UserInbox { get; init; } = null!;
    public int? FollowId { get; init; }
    public Follow? Follow { get; init; }
    public long? CommentId { get; init; }
    public Comment? Comment { get; init; }
    public int? MentionId { get; init; }
    public Mention? Mention { get; init; }
    public int? LikeId { get; init; }
    public Likes.Like? Like { get; init; }
    public int? CommentLikeId { get; init; }
    public CommentLike? CommentLike { get; init; }
    public bool IsDeleted { get; set; }
}



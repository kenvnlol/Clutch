using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.CommentThreads;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;
using Clutch.Features.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.Comments;

/// <summary>
/// A Comment is either a top-level comment or a reply.
/// 
/// - If it is a top-level comment, it has no parent and no root. 
///   (See: <see cref="CommentExtensions.IsTopLevelComment"/>)
/// - If it is a reply, it always has a parent and belongs to a thread. 
///   (See: <see cref="CommentExtensions.IsReply"/>)
/// </summary>
public class Comment : IClipInteraction, ICompactionKeyProvider, IInboxEvent
{
    public long Id { get; set; }
    public User Author { get; init; } = null!;
    public required string AuthorId { get; set; }
    public required string Text { get; init; }
    public int CommentThreadId { get; init; }
    public CommentThread CommentThread { get; init; } = null!;

    /// <summary>
    /// The top-level comment under a clip in which this comment belongs.
    /// Null if this is a top-level comment itself.
    /// Used to efficiently query all replies to a specific top-level comment.
    /// </summary>
    public long? RootCommentId { get; set; }
    public Comment? RootComment { get; set; }

    /// <summary>
    /// The ID of the comment to which this is a reply.
    /// Null if this is a top-level comment.
    /// Equals RootCommentId if this is a direct reply to the root comment.
    /// </summary>
    public long? ParentCommentId { get; set; }
    public Comment? ParentComment { get; set; }
    public int ReplyCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public required string DeviceId { get; init; }

    [NotMapped]
    public Clip Clip => CommentThread.Clip;

    [NotMapped]
    public int ClipId => CommentThread.ClipId;
    public required List<CommentLike> Likes { get; init; }
    public required List<Comment> Replies { get; init; }
    public required string Platform { get; init; }
    public required string BrowserLanguage { get; init; }
    public required string DevicePlatform { get; init; }
    public bool UserIsLoggedIn { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required InboxActivity InboxActivity { get; init; }
    public bool IsDeleted { get; set; }
    public required int EventId { get; init; }

    /// <summary>
    /// In-memory toggle state for cache/materializer:
    /// true = user posted; false = user undid comment (tombstone).
    /// Not persisted to the database.
    /// </summary>
    [NotMapped]
    public CommentType Type { get; init; }
}




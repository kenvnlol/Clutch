using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.Likes;

public class Like : IClipInteraction, ICompactionKeyProvider, IInboxEvent
{
    public int Id { get; set; }
    public required string DeviceId { get; init; }
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public int ClipId { get; init; }
    public Clip Clip { get; init; } = null!;
    public required string Platform { get; init; }
    public required string BrowserLanguage { get; init; }
    public required string DevicePlatform { get; init; }
    public bool UserIsLoggedIn { get; init; }
    public required DateTimeOffset Timestamp { get; set; }
    public required InboxActivity InboxActivity { get; set; }
    public bool IsDeleted { get; set; }

    /// <summary>
    /// In-memory toggle state for cache/materializer:
    /// true = user liked; false = user unliked (tombstone).
    /// Not persisted to the database.
    /// </summary>
    [NotMapped]
    public required LikeType LikeType { get; init; }

    public required int EventId { get; init; }
}
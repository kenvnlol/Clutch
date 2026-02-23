using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Database.Entities.Views;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Shared.Interfaces;

namespace Clutch.Database.Entities.UserEvents;

/// <summary>
/// A DTO of Like. The reason we are using a DTO here is because this item is immutable. The event must point at an immutable data point.
/// </summary>
/// <param name="ClipId"></param>
/// <param name="DeviceId"></param>
/// <param name="AuthorId"></param>
/// <param name="Platform"></param>
/// <param name="BrowserLanguage"></param>
/// <param name="DevicePlatform"></param>
/// <param name="UserIsLoggedIn"></param>
/// <param name="Timestamp"></param>
public record LikeEventData(
      int ClipId,
      string DeviceId,
      string AuthorId,
      string Platform,
      string BrowserLanguage,
      string DevicePlatform,
      bool UserIsLoggedIn,
      DateTimeOffset Timestamp,
      LikeType Type
  ) : ICompactionKeyProvider
{
    public Like ToEntity(int eventId, string inboxRecipient) => new Like
    {
        ClipId = ClipId,
        AuthorId = AuthorId,
        DeviceId = DeviceId,
        Platform = Platform,
        BrowserLanguage = BrowserLanguage,
        DevicePlatform = DevicePlatform,
        UserIsLoggedIn = UserIsLoggedIn,
        Timestamp = Timestamp,
        InboxActivity = new InboxActivity
        {
            Type = InboxNotificationType.ClipLike,
            Timestamp = Timestamp,
            InitiatorId = AuthorId,
            UserInboxId = inboxRecipient
        },
        IsDeleted = false,
        LikeType = Type,
        EventId = eventId
    };
}







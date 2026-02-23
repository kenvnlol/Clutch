
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Shared.Interfaces;

public record ShareEventData(
    int ClipId,
    string AuthorId,
    string DeviceId,
    string Platform,
    string BrowserLanguage,
    string DevicePlatform,
    ShareDestination Destination,
    bool UserIsLoggedIn,
    DateTimeOffset Timestamp
) : ICompactionKeyProvider;

public record FollowEventData(
    string InitiatorUserId,
    string TargetUserId,
    DateTimeOffset Timestamp,
    FollowType Type
) : ICompactionKeyProvider
{
    public Follow ToEntity(int eventId) => new Follow
    {
        InitiatorUserId = InitiatorUserId,
        TargetUserId = TargetUserId,
        Timestamp = Timestamp,
        InboxActivity = new InboxActivity
        {
            Type = InboxNotificationType.UserFollow,
            Timestamp = Timestamp,
            InitiatorId = InitiatorUserId,
            UserInboxId = TargetUserId
        },
        EventId = eventId,
        FollowType = Type
    };
}

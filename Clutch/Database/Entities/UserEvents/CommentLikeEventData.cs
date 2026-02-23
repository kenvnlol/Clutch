using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Shared.Interfaces;

public record CommentLikeEventData(
    long CommentId,
    string InitiatorUserId,
    DateTimeOffset Timestamp,
    LikeType Type
) : ICompactionKeyProvider
{
    public CommentLike ToEntity(int eventId, string inboxRecipient) => new CommentLike
    {
        CommentId = CommentId,
        InitiatorUserId = InitiatorUserId,
        Timestamp = Timestamp,
        InboxActivity = new InboxActivity
        {
            Type = InboxNotificationType.CommentLike,
            Timestamp = Timestamp,
            InitiatorId = InitiatorUserId,
            UserInboxId = inboxRecipient
        },
        IsDeleted = false,
        LikeType = Type,
        EventId = eventId
    };
}
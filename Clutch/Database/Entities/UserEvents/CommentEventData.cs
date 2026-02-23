

using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Interfaces;

public record CommentEventData(
        long CommentId,
        int CommentThreadId,
        long? RootCommentId,
        long? ParentCommentId,
        string AuthorId,
        string Text,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform,
        bool UserIsLoggedIn,
        DateTimeOffset Timestamp,
        CommentType Type
    ) : ICompactionKeyProvider
{

    public Comment ToEntity(int eventId, string inboxRecipient) => new Comment
    {
        Id = CommentId,
        CommentThreadId = CommentThreadId,
        RootCommentId = RootCommentId,
        ParentCommentId = ParentCommentId,
        AuthorId = AuthorId,
        Text = Text,
        DeviceId = DeviceId,
        Platform = Platform,
        BrowserLanguage = BrowserLanguage,
        DevicePlatform = DevicePlatform,
        UserIsLoggedIn = UserIsLoggedIn,
        Timestamp = Timestamp,
        InboxActivity = new InboxActivity
        {
            Type = InboxNotificationType.CommentClip,
            Timestamp = Timestamp,
            InitiatorId = AuthorId,
            UserInboxId = inboxRecipient
        },
        IsDeleted = false,
        EventId = eventId,
        Likes = [],
        Replies = [],
        Type = Type
    };
}
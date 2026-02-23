using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.UserInboxes;

public class UserInbox
{
    public string UserId { get; init; } = null!;
    public User User { get; init; } = null!;
    public required List<InboxActivity> Activities { get; init; }
    public required int UnseenCommentLikeCount { get; set; }
    public required int UnseenCommentReplyCount { get; set; }
    public required int UnseenCommentClipCount { get; set; }
    public required int UnseenUserMentionCount { get; set; }
    public required int UnseenUserFollowCount { get; set; }
    public required int UnseenClipLikeCount { get; set; }
    public required int UnseenDirectMessageCount { get; set; }
}



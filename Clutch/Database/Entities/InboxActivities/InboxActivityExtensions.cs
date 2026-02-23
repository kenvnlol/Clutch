using Clutch.Database.Entities.UserInboxes;

namespace Clutch.Database.Entities.InboxActivities;

public static class InboxActivityExtensions
{
    public static string GetDisplayMessage(this InboxActivity activity)
    {
        if (!Messages.TryGetValue(activity.Type, out var template))
        {
            throw new InvalidOperationException($"Missing message template for {activity.Type}");
        }

        return string.Format(template, activity.Initiator.DisplayName, activity.Comment?.Text ?? "");
    }


    public static string? GetResourceAvatar(this InboxActivity activity)
        => activity.Type switch
        {
            InboxNotificationType.CommentClip => activity.Comment?.Clip?.Media.Avatar.BlobName,
            InboxNotificationType.ClipLike => activity.Like?.Clip?.Media.Avatar.BlobName,
            InboxNotificationType.CommentLike => activity.CommentLike?.Comment.Clip.Media.Avatar.BlobName,
            _ => null
        };


    private static readonly Dictionary<InboxNotificationType, string> Messages = new()
    {
        { InboxNotificationType.UserFollow, "{0} started following you." },
        { InboxNotificationType.CommentReply, "{0} replied to your comment: {1}" },
        { InboxNotificationType.CommentClip, "{0} commented: {1}" },
        { InboxNotificationType.UserMention, "{0} mentioned you in a comment: {1}" },
        { InboxNotificationType.ClipLike, "{0} liked your clip." }
    };
}
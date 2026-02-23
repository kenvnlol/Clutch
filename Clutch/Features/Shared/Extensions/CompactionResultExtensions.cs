using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Extensions;
using Clutch.Features.Shared.Interfaces;
using System.Diagnostics;

public static class CompactionResultExtensions
{
    public static EntityProcessingResult<Y> GetEventActionDetails<T, Y>(
        this CompactionResult<T> ev,
        List<Y> existing)
        where T : class
        where Y : class, ICompactionKeyProvider
    {
        var match = existing.FirstOrDefault(e =>
            e.GetCompactionKey() == UserEventExtensions.GetCompactionKey(ev.Data));

        if (ev.Type == EventActionType.Create && match is null)
        {
            return new(EntityAction.Add);
        }

        if (ev.Type == EventActionType.Delete && match is not null)
        {
            return new(EntityAction.Delete, match);
        }

        return new(EntityAction.Skip);
    }


    public static UserInbox ResolveUserEventInbox<T>(
     this CompactionResult<T> compactedEvent,
     List<UserInbox> inboxes)
     where T : class
    {
        return compactedEvent.Data switch
        {
            LikeEventData le => inboxes
                .First(inbox => inbox.User.Clips.Any(c => c.Id == le.ClipId)),

            CommentLikeEventData cle => inboxes
                .First(inbox => inbox.User.Comments.Any(c => c.Id == cle.CommentId)),

            CommentEventData ce => ResolveCommentInbox(ce, inboxes),

            FollowEventData fe => inboxes
                .First(inbox => inbox.UserId == fe.TargetUserId),

            _ => throw new UnreachableException($"{nameof(T)} is not supported to resolve inbox events. ")
        };
    }

    private static UserInbox ResolveCommentInbox(
        CommentEventData ce,
        List<UserInbox> inboxes)
    {
        // Try to find the parent comment author first
        var parentInbox = ce.ParentCommentId is { } parentId
            ? inboxes.First(inbox => inbox.User.Comments.Any(c => c.Id == parentId))
            : null;

        if (parentInbox != null)
            return parentInbox;

        // Otherwise, try to find the author of the clip thread
        return inboxes.First(inbox => inbox.User.Clips.Any(c => c.Id == ce.CommentThreadId));
    }
}

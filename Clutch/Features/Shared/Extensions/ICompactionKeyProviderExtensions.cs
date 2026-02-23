using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.Views;
using Clutch.Features.Shared.Interfaces;

namespace Clutch.Features.Shared.Extensions;

public static class CompactionKeyProviderExtensions
{
    public static string GetCompactionKey(this ICompactionKeyProvider provider)
        => provider switch
        {
            LikeEventData e => $"{e.AuthorId}:{e.ClipId}",
            Like l => $"{l.AuthorId}:{l.ClipId}",

            ViewEventData e => $"{e.AuthorId ?? e.DeviceId}:{e.ClipId}",
            View v => $"{v.AuthorId ?? v.DeviceId}:{v.ClipId}",

            CommentEventData e => $"{e.AuthorId}:{e.CommentId}",
            Comment c => $"{c.AuthorId}:{c.Id}",

            CommentLikeEventData e => $"{e.InitiatorUserId}:{e.CommentId}",
            CommentLike cl => $"{cl.InitiatorUserId}:{cl.CommentId}",

            SaveEventData e => $"{e.AuthorId}:{e.ClipId}",
            Save s => $"{s.AuthorId}:{s.ClipId}",

            FollowEventData e => $"{e.InitiatorUserId}:{e.TargetUserId}",
            Follow f => $"{f.InitiatorUserId}:{f.TargetUserId}",

            _ => throw new InvalidOperationException(
                    $"No compaction-key defined for {provider.GetType().Name}"
                 )
        };
}

using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Extensions;
using CommunityToolkit.Diagnostics;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.CommentLikes.Services;


[Handler]
public sealed partial class MaterializeCommentLikes(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.CommentLikeEntity);
 
        if (lastOffset == null)
        {
            return;
        }


        var compactedCommentLikes = await dbContext.UserEvents
            .FetchAndCompactFromOffsetAsync<CommentLikeEventData>(lastOffset);

        Materialize(
            compactedCommentLikes,
            await GetExistingAsync(
                compactedCommentLikes,
                dbContext,
                cancellationToken),
            await FetchUserInboxesAsync(
                compactedCommentLikes,
                dbContext,
                cancellationToken),
            dbContext);

        lastOffset.AdvanceTo(compactedCommentLikes);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<CommentLike>> GetExistingAsync(
        List<CompactionResult<CommentLikeEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.CommentLikes
            .Where(cl => keys.Any(k => k == (cl.InitiatorUserId + ":" + cl.CommentId)))
            .Include(cl => cl.InboxActivity)
            .ToListAsync();

        return query;
    }
    public static Task<List<UserInbox>> FetchUserInboxesAsync(
        List<CompactionResult<CommentLikeEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var likedCommentIds = compactedBatch
            .Select(cb => cb.Data.CommentId)
            .ToHashSet();

        return dbContext.UserInboxes
            .AsTracking()
            .AsSplitQuery()
            .Where(inbox => inbox.User.Comments.Any(c => likedCommentIds.Contains(c.Id)))
            .Include(inbox => inbox.User).ThenInclude(user => user.Clips)
            .Include(inbox => inbox.User).ThenInclude(user => user.Comments)
            .ToListAsync(cancellationToken);
    }

    private static void Materialize(
    List<CompactionResult<CommentLikeEventData>> compactedLikes,
    List<CommentLike> existingLikes,
    List<UserInbox> userInboxes,
    ApplicationDbContext dbContext)
    {
        foreach (var ev in compactedLikes)
        {
            var result = ev.GetEventActionDetails(existingLikes);

            switch (result.Action)
            {
                case EntityAction.Add:
                    var inboxToAdd = ev.ResolveUserEventInbox(userInboxes);
                    dbContext.CommentLikes.Add(ev.Data.ToEntity(ev.EventId, inboxToAdd.UserId));
                    break;

                case EntityAction.Delete:
                    Guard.IsNotNull(result.ExistingMatch);
                    var inboxToDelete = ev.ResolveUserEventInbox(userInboxes);
                    inboxToDelete.UnseenCommentLikeCount = Math.Max(0, inboxToDelete.UnseenCommentLikeCount - 1);
                    dbContext.CommentLikes.Remove(result.ExistingMatch);
                    dbContext.InboxActivities.Remove(result.ExistingMatch.InboxActivity);
                    break;

                case EntityAction.Skip:
                    break;

                default:
                    break;
            }
        }
    }
}

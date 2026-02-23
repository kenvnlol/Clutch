using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Extensions;
using CommunityToolkit.Diagnostics;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.CommentThreads.Services;

[Handler]
public sealed partial class MaterializeComments(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.CommentEntity);


        if (lastOffset == null)
        {
            return;
        }

        var compactedComments = await dbContext.UserEvents.FetchAndCompactFromOffsetAsync<CommentEventData>(lastOffset);


        Materialize(
            compactedComments,
            await GetExistingAsync(
                compactedComments,
                dbContext,
                cancellationToken),
            await FetchUserInboxesAsync(
                compactedComments,
                dbContext,
                cancellationToken),
            dbContext);

        lastOffset.AdvanceTo(compactedComments);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<Comment>> GetExistingAsync(
        List<CompactionResult<CommentEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.Comments
            .Where(c => keys.Any(k => k == (c.AuthorId + ":" + c.Id)))
            .Include(l => l.InboxActivity)
            .ToListAsync();

        return query;
    }
    public static Task<List<UserInbox>> FetchUserInboxesAsync(
        List<CompactionResult<CommentEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var commentedClipIds = compactedBatch
            .Where(cb => cb.Data.RootCommentId is null && cb.Data.ParentCommentId is null)
            .Select(cb => cb.Data.CommentThreadId!)
            .ToHashSet();

        var repliedToCommentIds = compactedBatch
            .Where(cb => cb.Data.ParentCommentId != null)
            .Select(cb => cb.Data.ParentCommentId!.Value)
            .ToHashSet(); // HashSet<long>

        return dbContext.UserInboxes
            .AsTracking()
            .AsSplitQuery()
            .Where(y =>
                y.User.Clips.Any(c => commentedClipIds.Contains(c.Id)) ||
                y.User.Comments.Any(c => repliedToCommentIds.Contains(c.Id)))
            .Include(y => y.User).ThenInclude(u => u.Clips)
            .Include(y => y.User).ThenInclude(u => u.Comments)
            .ToListAsync();
    }

    public static void Materialize(
          List<CompactionResult<CommentEventData>> compactedComments,
          List<Comment> existingComments,
          List<UserInbox> userInboxes,
          ApplicationDbContext dbContext)
    {
        foreach (var ev in compactedComments)
        {
            var result = ev.GetEventActionDetails(existingComments);

            switch (result.Action)
            {
                case EntityAction.Add:
                    var inboxToAdd = ev.ResolveUserEventInbox(userInboxes);
                    if (ev.Data.ParentCommentId.HasValue)
                    {
                        inboxToAdd.UnseenCommentReplyCount++;
                    }
                    else
                    {
                        inboxToAdd.UnseenCommentClipCount++;
                    }

                    dbContext.Comments.Add(ev.Data.ToEntity(ev.EventId, inboxToAdd.UserId));
                    break;

                case EntityAction.Delete:
                    Guard.IsNotNull(result.ExistingMatch);
                    var existing = result.ExistingMatch;
                    var inboxToDelete = ev.ResolveUserEventInbox(userInboxes);

                    if (ev.Data.ParentCommentId.HasValue)
                    {
                        inboxToDelete.UnseenCommentReplyCount = Math.Max(0, inboxToDelete.UnseenCommentReplyCount - 1);
                    }
                    else
                    {
                        inboxToDelete.UnseenCommentClipCount = Math.Max(0, inboxToDelete.UnseenCommentClipCount - 1);
                    }

                    dbContext.Comments.Remove(existing);
                    dbContext.InboxActivities.Remove(existing.InboxActivity);
                    break;

                case EntityAction.Skip:
                    break;
            }
        }
    }

}


public static class CommentExtensions
{
    public static bool IsReply(this CommentEventData data) =>
        data.ParentCommentId != null;

    public static bool IsReply(this Comment comment) =>
        comment.ParentCommentId != null;
}

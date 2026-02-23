using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Extensions;
using CommunityToolkit.Diagnostics;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.Likes.Services;

[Handler]
public sealed partial class MaterializeLikes(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.LikeEntity);

        if(lastOffset == null)
        {
            return;
        }

        var compactedLikes = await dbContext.UserEvents
            .FetchAndCompactFromOffsetAsync<LikeEventData>(lastOffset);


        Materialize(
            compactedLikes,
            await GetExistingAsync(
                compactedLikes,
                dbContext,
                cancellationToken),
            await FetchUserInboxesAsync(
                compactedLikes,
                dbContext,
                cancellationToken),
            dbContext);

        lastOffset.AdvanceTo(compactedLikes);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<Like>> GetExistingAsync(
        List<CompactionResult<LikeEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.Likes
            .Where(c => keys.Any(k => k == (c.AuthorId + ":" + c.ClipId)))
            .Include(l => l.InboxActivity)
            .ToListAsync();

        return query;
    }
    public static Task<List<UserInbox>> FetchUserInboxesAsync(
        List<CompactionResult<LikeEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var clipIds = compactedBatch
            .Select(cb => cb.Data.ClipId)
            .ToHashSet();

        return dbContext.UserInboxes
            .AsTracking()
            .AsSplitQuery()
            .Where(inbox => inbox.User.Clips.Any(c => clipIds.Contains(c.Id)))
            .Include(inbox => inbox.User).ThenInclude(user => user.Clips)
            .Include(inbox => inbox.User).ThenInclude(user => user.Comments)
            .ToListAsync(cancellationToken);
    }

    private static void Materialize(
    List<CompactionResult<LikeEventData>> compactedLikes,
    List<Like> existingLikes,
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
                    inboxToAdd.UnseenClipLikeCount++;
                    dbContext.Likes.Add(ev.Data.ToEntity(ev.EventId, inboxToAdd.UserId));
                    break;

                case EntityAction.Delete:
                    Guard.IsNotNull(result.ExistingMatch);
                    var inboxToDelete = ev.ResolveUserEventInbox(userInboxes);
                    inboxToDelete.UnseenClipLikeCount = Math.Max(0, inboxToDelete.UnseenClipLikeCount - 1);
                    dbContext.Likes.Remove(result.ExistingMatch);
                    dbContext.InboxActivities.Remove(result.ExistingMatch.InboxActivity);
                    break;

                case EntityAction.Skip:
                    break;
            }
        }
    }
}
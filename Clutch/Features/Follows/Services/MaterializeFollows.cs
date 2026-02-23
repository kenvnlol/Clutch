using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Follows;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Shared.Extensions;
using CommunityToolkit.Diagnostics;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.Follows.Services;

[Handler]
public sealed partial class MaterializeFollows(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.FollowEntity);

        if (lastOffset == null)
        {
            return;
        }

        var compactedFollows = await dbContext.UserEvents.FetchAndCompactFromOffsetAsync<FollowEventData>(lastOffset);

        Materialize(
            compactedFollows,
            await GetExistingAsync(
                compactedFollows,
                dbContext,
                cancellationToken),
            await FetchUserInboxesAsync(
                compactedFollows,
                dbContext,
                cancellationToken),
            dbContext);

        lastOffset.AdvanceTo(compactedFollows);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<Follow>> GetExistingAsync(
        List<CompactionResult<FollowEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.Follows
            .Where(f => keys.Any(k => k == (f.InitiatorUserId + ":" + f.TargetUserId)))
            .Include(f => f.InboxActivity)
            .ToListAsync();

        return query;
    }
    public static Task<List<UserInbox>> FetchUserInboxesAsync(
        List<CompactionResult<FollowEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var userIds = compactedBatch.Select(cb => cb.Data.TargetUserId).ToHashSet();

        return dbContext.UserInboxes
            .AsTracking()
            .AsSplitQuery()
            .Where(y => userIds.Contains(y.UserId))
            .ToListAsync();
    }

    private static void Materialize(
    List<CompactionResult<FollowEventData>> compactedFollows,
    List<Follow> existingFollows,
    List<UserInbox> userInboxes,
    ApplicationDbContext dbContext)
    {
        foreach (var ev in compactedFollows)
        {
            var result = ev.GetEventActionDetails(existingFollows);

            switch (result.Action)
            {
                case EntityAction.Add:
                    var inboxToAdd = ev.ResolveUserEventInbox(userInboxes);
                    inboxToAdd.UnseenUserFollowCount++;
                    dbContext.Follows.Add(ev.Data.ToEntity(ev.EventId));
                    break;

                case EntityAction.Delete:
                    Guard.IsNotNull(result.ExistingMatch);
                    var inboxToDelete = ev.ResolveUserEventInbox(userInboxes);
                    inboxToDelete.UnseenUserFollowCount = Math.Max(0, inboxToDelete.UnseenUserFollowCount - 1);
                    dbContext.Follows.Remove(result.ExistingMatch);
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


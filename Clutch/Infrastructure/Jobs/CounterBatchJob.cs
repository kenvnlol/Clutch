using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Infrastructure.Jobs;

public class CounterBatchJob(
    ApplicationDbContext dbContext)
    : IRecurringJob
{
    public async Task Execute(CancellationToken cancellationToken)
    {
        var lastOffset = await dbContext.EventConsumerOffsets
                .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.StatUpdater);

        if(lastOffset is null)
        {
            return;
        }

        var unprocessedEvents = await dbContext.UserEvents
            .AsTracking()
            .Where(ev => ev.Id > lastOffset.LastProcessedEventId)
            .ToListAsync();

        var compactedLikes = unprocessedEvents.CompactBy(ev => ev.LikeEvent);
        var compactedCommentLikes = unprocessedEvents.CompactBy(ev => ev.CommentLikeEvent);
        var compactedComments = unprocessedEvents.CompactBy(ev => ev.CommentEvent);
        var compactedViews = unprocessedEvents.CompactBy(ev => ev.ViewEvent);
        var compactedSaves = unprocessedEvents.CompactBy(ev => ev.SaveEvent);
        var compactedFollows = unprocessedEvents.CompactBy(ev => ev.FollowEvent);

        var clips = FetchClips(compactedLikes, compactedComments, compactedViews, compactedSaves);
        var comments = FetchComments(compactedComments, compactedCommentLikes);
        var users = FetchUsers(compactedFollows);

        if (unprocessedEvents.Count == 0)
        {
            return;
        }

        lastOffset.LastProcessedEventId = unprocessedEvents.Max(e => e.Id);
        await dbContext.SaveChangesAsync();
    }

    private Task<List<Clip>> FetchClips(
        IEnumerable<CompactionResult<LikeEventData>> likes,
        IEnumerable<CompactionResult<CommentEventData>> comments,
        IEnumerable<CompactionResult<ViewEventData>> views,
        IEnumerable<CompactionResult<SaveEventData>> saves)
    {
        var clipIds = likes.Select(x => x.Data.ClipId)
            .Concat(views.Select(x => x.Data.ClipId))
            .Concat(saves.Select(x => x.Data.ClipId))
            .Concat(comments
                .Where(c => c.Data.RootCommentId == null && c.Data.ParentCommentId == null)
                .Select(c => c.Data.CommentThreadId)
            )
            .ToHashSet();

        return dbContext.Clips
            .Where(c => clipIds.Contains(c.Id))
            .ToListAsync();
    }

    public void UpdateLikes(
        Dictionary<int, Clip> clips,
        IEnumerable<CompactionResult<LikeEventData>> compactedLikes)
    {
        var deltas = compactedLikes
            .GroupBy(b => b.Data.ClipId)
            .Select(g => new
            {
                ClipId = g.Key,
                Delta = g.Sum(b => b.Type == EventActionType.Create ? 1 : -1)
            });

        foreach (var d in deltas)
        {
            if (clips.TryGetValue(d.ClipId, out var clip))
            {
                clip.LikeCount += d.Delta;
            }
        }
    }

    private Task<List<Comment>> FetchComments(
        IEnumerable<CompactionResult<CommentEventData>> comments,
        IEnumerable<CompactionResult<CommentLikeEventData>> commentLikes)
    {
        var commentIds = comments
            .Select(c => c.Data.RootCommentId ?? c.Data.ParentCommentId) // Replies target a root or parent
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .Concat(commentLikes.Select(cl => cl.Data.CommentId))
            .ToHashSet();

        return dbContext.Comments
            .Where(c => commentIds.Contains(c.Id))
            .ToListAsync();
    }

    private Task<List<User>> FetchUsers(
        IEnumerable<CompactionResult<FollowEventData>> follows)
    {
        var userIds = follows
            .Select(f => f.Data.TargetUserId)
            .Concat(follows.Select(f => f.Data.InitiatorUserId))
            .ToHashSet();


        return dbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync();
    }
}



using Clutch.Database;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.CommentThreads.Endpoints;

[Handler]
[MapGet("/comment-thread")]
public static partial class GetCommentThread
{
    public sealed record Query(
        int ClipId,
        int LikeCursor = int.MaxValue,
        int CommentCursor = int.MaxValue
        );

    public sealed record CommentThreadDto(
        int Id,
        int Total,
        int? LikeCursor,
        long? CommentCursor,
        List<TopLevelComment> Comments
    );

    public sealed record TopLevelComment(
        long Id,
        UserDto Author,
        bool IsPinned,
        string Text,
        DateTimeOffset CreatedAt,
        int ReplyCount,
        int LikeCount
    );

    public sealed record UserDto(
        string Id,
        string DisplayName,
        string DisplayImage
    );

    private static async ValueTask<CommentThreadDto> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        CancellationToken token
    )
    {
        var thread = await dbContext.CommentThreads
            .AsSplitQuery()
            .AsNoTracking()
            .Include(commentThread => commentThread.Clip)
            .Include(commentThread => commentThread.Comments
                .OrderByDescending(comment => comment.LikeCount)
                .Where(comment => comment.RootCommentId == null)
                .Where(comment =>
                       comment.LikeCount < query.LikeCursor ||
                       (comment.LikeCount == query.LikeCursor && comment.Id < query.CommentCursor))
                .Take(20))
            .ThenInclude(x => x.Author)
            .FirstAsync(x => x.ClipId == query.ClipId);

        return new CommentThreadDto(
            thread.ClipId,
            thread.Clip.CommentCount,
            thread.Comments.LastOrDefault()?.LikeCount ?? null,
            thread.Comments.LastOrDefault()?.Id ?? null,
            thread.Comments
                .Select(comment => new TopLevelComment(
                    comment.Id,
                    new UserDto(
                        comment.Author.Id,
                        comment.Author.DisplayName,
                        comment.Author.AvatarUri
                    ),
                    false,
                    comment.Text,
                    comment.Timestamp,
                    comment.ReplyCount,
                    comment.LikeCount
                ))
                .ToList()
                );
    }
}

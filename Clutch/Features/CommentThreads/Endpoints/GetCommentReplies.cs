using Clutch.Database;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.CommentThreads.Endpoints;

[Handler]
[MapGet("/comment-thread/comment/replies")]
public static partial class GetCommentReplies
{
    public sealed record Query(
        long CommentId);

    public sealed record ReplyDto(
        long Id,
        UserDto Author,
        bool IsPinned,
        string Text,
        DateTimeOffset CreatedAt,
        int ReplyCount,
        int LikeCount,
        long ParentCommentId,
        long? ReplyToReplyId
    );

    public sealed record UserDto(
        string Id,
        string DisplayName,
        string DisplayImage
    );


    private static async ValueTask<IReadOnlyList<ReplyDto>> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        CancellationToken token)
    {
        var replies = await dbContext.Comments
            .Where(c => c.RootCommentId == query.CommentId)
            .Include(r => r.Author) // if you want user details, etc.
            .ToListAsync();

        return replies.Select(reply
            => new ReplyDto(
                reply.Id,
                new UserDto(
                        reply.Author.Id,
                        reply.Author.DisplayName,
                        reply.Author.AvatarUri),
                false,
                reply.Text,
                reply.Timestamp,
                reply.ReplyCount,
                reply.LikeCount,
                query.CommentId,
                reply.ParentCommentId)).ToList();
    }
}

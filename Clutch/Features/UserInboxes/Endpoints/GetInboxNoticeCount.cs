using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

[Handler]
[MapGet("/user/inbox/count")]
public static partial class GetInboxNoticeCount
{
    public sealed record Query;
    public record Response(
         int UnseenCommentLikeCount,
         int UnseenCommentReplyCount,
         int UnseenCommentClipCount,
         int UnseenUserMentionCount,
         int UnseenUserFollowCount,
         int UnseenClipLikeCount,
         int UnseenDirectMessageCount
         );

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
            => await dbContext.UserInboxes
                .Where(inbox =>
                    inbox.UserId == userService.GetCurrentUserId())
                .Select(inbox => new Response(
                    inbox.UnseenCommentLikeCount,
                    inbox.UnseenCommentReplyCount,
                    inbox.UnseenCommentClipCount,
                    inbox.UnseenUserMentionCount,
                    inbox.UnseenUserFollowCount,
                    inbox.UnseenClipLikeCount,
                    inbox.UnseenDirectMessageCount))
                .FirstAsync();
}

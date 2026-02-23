using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Users.Endpoints;
[Handler]
[MapGet("/user/inbox/interactions/seen")]
public static partial class MarkInteractionsInboxAsSeen
{
    public sealed record Query;

    private static async ValueTask HandleAsync(
        Query _,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
            => await dbContext.UserInboxes
            .Where(u => u.UserId == userService.GetCurrentUserId())
            .ExecuteUpdateAsync(updates => updates
                .SetProperty(u => u.UnseenCommentLikeCount, 0)
                .SetProperty(u => u.UnseenCommentReplyCount, 0)
                .SetProperty(u => u.UnseenCommentClipCount, 0)
                .SetProperty(u => u.UnseenUserMentionCount, 0)
                .SetProperty(u => u.UnseenUserFollowCount, 0)
                .SetProperty(u => u.UnseenClipLikeCount, 0),
                token);

}


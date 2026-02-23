using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Users.Endpoints;

[Handler]
[MapGet("/user/inbox/direct-conversations/seen")]
public static partial class MarkDirectConversationsInboxAsSeen
{
    public sealed record Query;
    private static async ValueTask<int> HandleAsync(
        Query _,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
        => await dbContext.UserInboxes
            .Where(inbox => inbox.UserId == userService.GetCurrentUserId())
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(inbox => inbox.UnseenDirectMessageCount, 0),
                cancellationToken: token);

}

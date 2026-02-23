using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.DirectThreads.Endpoints;

[Handler]
[MapDelete("/direct-thread/message")]
// TODO: signalr on the message.
public static partial class UnsendMessage
{
    public sealed record Query(
        int Id);

    private static async ValueTask HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
            => await dbContext.DirectMessages.Where(dm =>
            dm.AuthorId == userService.GetCurrentUserId())
            .ExecuteUpdateAsync(setter =>
                setter.SetProperty(message =>
                    message.IsDeleted, true));
}

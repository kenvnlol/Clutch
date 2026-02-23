using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.DirectThreads.Endpoints;

[Handler]
[MapPost("/user/direct-thread/read")]
public static partial class MarkDirectThreadAsRead
{
    public sealed record Command(
        int DirectConversationId);

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var utc = DateTimeOffset.UtcNow;

        await dbContext.DirectThreads
            .Where(thread => thread.Id == command.DirectConversationId)
            .Where(thread => thread.ParticipantAId == currentUserId || thread.ParticipantBId == currentUserId)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(
                    thread => thread.ParticipantALastReadAt,
                    thread => thread.ParticipantAId == currentUserId ? utc : thread.ParticipantALastReadAt
                )
                .SetProperty(
                    thread => thread.ParticipantBLastReadAt,
                    thread => thread.ParticipantBId == currentUserId ? utc : thread.ParticipantBLastReadAt
                ),
                token
            );
    }
}

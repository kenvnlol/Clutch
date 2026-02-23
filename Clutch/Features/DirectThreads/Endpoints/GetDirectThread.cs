using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.DirectThreads.Endpoints;

[Handler]
[MapGet("/user/direct-thread")]
// TODO: reusing the thread query here
public static partial class GetDirectThread
{
    public sealed record Command(
        int ThreadId,
        int Cursor = 0);

    public sealed record Response(
        DateTimeOffset? OtherParticipantLastReadAt,
        IReadOnlyList<DirectMessageDto> Messages,
        int? NextCursor);

    public sealed record DirectMessageDto(
        int Id,
        DateTimeOffset Timestamp,
        DateTimeOffset? ReadAt,
        ThreadAuthor ThreadAuthor,
        string Text
    );

    public sealed record ThreadAuthor(
        string SenderId,
        string DisplayName,
        string AvatarUri
    );

    private static async ValueTask<Response> HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService user,
        CancellationToken token)
    {
        var currentUserId = user.GetCurrentUserId();

        var thread = await dbContext.DirectThreads
            .Where(thread => thread.Id == command.ThreadId)
            .Where(thread => thread.ParticipantAId == currentUserId || thread.ParticipantBId == currentUserId)
            .Select(thread => new
            {
                OtherParticipantLastReadAt = thread.ParticipantAId == currentUserId
                    ? thread.ParticipantBLastReadAt
                    : thread.ParticipantALastReadAt,
                Messages = thread.Messages
                    .Where(m => command.Cursor == default || m.Id < command.Cursor) // Handle first request case
                    .OrderByDescending(m => m.Id)
                    .Take(5)
                    .Select(message => new DirectMessageDto(
                        message.Id,
                        message.Timestamp,
                        message.ReadAt,
                        new ThreadAuthor(
                            message.Author.Id,
                            message.Author.DisplayName,
                            message.Author.AvatarUri
                        ),
                        message.Text
                    ))
                    .ToList()
            })
            .FirstAsync(token);

        var nextCursor = thread.Messages.LastOrDefault()?.Id;

        return new Response(thread.OtherParticipantLastReadAt, thread.Messages, nextCursor);
    }
}
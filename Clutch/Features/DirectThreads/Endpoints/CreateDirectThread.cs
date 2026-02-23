using Clutch.Database;
using Clutch.Database.Entities.DirectThreads;
using Clutch.Features.Shared.Extensions;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.DirectThreads.Endpoints;

[Handler]
[MapPost("/user/direct-thread")]
// TODO: race condition on creating thread.
public static partial class CreateDirectThread
{
    public sealed record Command(
        string RecipientUserId);

    public sealed record Response(
        int ThreadId,
        DateTimeOffset? OtherParticipantLastReadAt,
        IReadOnlyList<DirectMessageDto> Messages);

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
        UserService userService,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        if (currentUserId == command.RecipientUserId)
        {
            throw new InvalidOperationException("Cannot message yourself.");
        }

        var orderedParticipants = (currentUserId, command.RecipientUserId).Sort();

        var thread = await dbContext.DirectThreads
            .Where(c => c.ParticipantAId == orderedParticipants.Item1 && c.ParticipantBId == orderedParticipants.Item2)
            .Select(c => new
            {
                c.Id,
                OtherParticipantLastReadAt = c.ParticipantAId == currentUserId
                    ? c.ParticipantBLastReadAt
                    : c.ParticipantALastReadAt,
                Messages = c.Messages
                    .OrderByDescending(m => m.Timestamp)
                    .Take(20)
                    .Select(m => new DirectMessageDto(
                        m.Id,
                        m.Timestamp,
                        m.ReadAt,
                        new ThreadAuthor(
                            m.Author.Id,
                            m.Author.DisplayName,
                            m.Author.AvatarUri
                        ),
                        m.Text
                    ))
                    .ToList()
            })
            .FirstOrDefaultAsync(token);

        if (thread is not null)
        {
            return new Response(thread.Id, thread.OtherParticipantLastReadAt, thread.Messages);
        }

        var newThread = new DirectThread
        {
            ParticipantAId = orderedParticipants.Item1,
            ParticipantBId = orderedParticipants.Item2,
            Messages = []
        };

        dbContext.DirectThreads.Add(newThread);

        await dbContext.SaveChangesAsync(token);
        return new Response(newThread.Id, null, new List<DirectMessageDto>());
    }
}

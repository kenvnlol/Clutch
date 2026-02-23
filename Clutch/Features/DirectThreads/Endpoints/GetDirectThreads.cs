using Clutch.Database;
using Clutch.Database.Entities.DirectThreads;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.DirectThreads.Endpoints;

[Handler]
[MapGet("/user/direct-threads")]
public static partial class GetDirectThreads
{
    public sealed record Query;
    public sealed record Response(
        IReadOnlyList<DirectThreadDto> Threads);

    public sealed record DirectThreadDto(
        int Id,
        DateTimeOffset LastMessageAt,
        string LastMessage,
        Recipient Recipient,
        bool HasUnreadMessages);

    public sealed record Recipient(
        string UserName,
        string AvatarUri);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var userConversations = await dbContext.DirectThreads
            .Where(thread => thread.ParticipantAId == currentUserId || thread.ParticipantBId == currentUserId)
            .Where(thread => thread.LastMessage != null)
            .AsSplitQuery()
            .OrderByDescending(thread => thread.LastMessageAt)
            .Include(thread => thread.ParticipantA)
            .Include(thread => thread.ParticipantB)
            .ToListAsync(token);

        if (userConversations.Count == 0)
        {
            return new Response([]);
        }

        return new Response(userConversations
            .Select(conversation =>
            {
                var recipient = conversation.ParticipantAId == currentUserId
                    ? conversation.ParticipantB
                    : conversation.ParticipantA;

                return new DirectThreadDto(
                    conversation.Id,
                    conversation.LastMessageAt!.Value,
                    conversation.LastMessage!,
                    new Recipient(recipient.UserName!, recipient.AvatarUri),
                    HasUnreadMessages(conversation, currentUserId));
            })
            .ToList());
    }

    private static bool HasUnreadMessages(DirectThread conversation, string currentUserId)
    {
        var participantLastReadAt = currentUserId == conversation.ParticipantAId
            ? conversation.ParticipantALastReadAt
            : conversation.ParticipantBLastReadAt;

        return conversation.LastMessageAt > (participantLastReadAt ?? DateTimeOffset.MinValue);
    }
}

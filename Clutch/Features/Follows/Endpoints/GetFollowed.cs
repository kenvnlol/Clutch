using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Follows.Endpoints;

[Handler]
[MapGet("/user/followed")]
public static partial class GetFollowed
{
    public sealed record Query(
        string UserId,
        int Cursor = 0);

    public sealed record Response(
        List<FollowedDto> Users,
        int? Cursor);

    public sealed record FollowedDto(
        int Id,
        string UserId,
        string DisplayName,
        string AvatarUri,
        DateTimeOffset Timestamp);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var followed = await dbContext.Follows
            .Where(follow => follow.InitiatorUserId == currentUserId)
            .Where(follow => query.Cursor == default || follow.Id < query.Cursor) // Cursor-based pagination
            .OrderByDescending(f => f.Id) // Ensuring proper cursor order
            .Take(20)
            .Select(follow => new FollowedDto(
                follow.Id,
                follow.TargetUserId,
                follow.TargetUser.DisplayName,
                follow.TargetUser.AvatarUri,
                follow.Timestamp
            ))
            .ToListAsync(token);

        var nextCursor = followed.LastOrDefault()?.Id ?? null;

        return new Response(followed, nextCursor);
    }
}

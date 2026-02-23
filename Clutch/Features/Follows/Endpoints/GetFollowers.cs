using Clutch.Database;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Follows.Endpoints;

[Handler]
[MapGet("/user/followers")]
public static partial class GetFollowers
{
    public sealed record Query(
        string UserId,
        int Cursor = 0);

    public sealed record Response(
        List<FollowerDto> Users,
        int? Cursor);

    public sealed record FollowerDto(
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

        var followers = await dbContext.Follows
            .Where(follow => follow.TargetUserId == currentUserId)
            .Where(follow => query.Cursor == default || follow.Id < query.Cursor) // Cursor-based pagination
            .OrderByDescending(f => f.Id) // Ensuring proper cursor order
            .Take(20)
            .Select(follow => new FollowerDto(
                follow.Id,
                follow.InitiatorUserId,
                follow.InitiatorUser.DisplayName,
                follow.InitiatorUser.AvatarUri,
                follow.Timestamp
            ))
            .ToListAsync(token);

        var nextCursor = followers.LastOrDefault()?.Id ?? null;

        return new Response(followers, nextCursor);
    }
}



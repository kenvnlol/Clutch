using Clutch.Database;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Users.Endpoints;

[Handler]
[MapGet("/user/profile")]
// TODO: add friendcount, verified, private profile etc...
// TODO: Maybe add user CLIP count and user LIKED to make pagination easier. 
public static partial class GetProfile
{
    public sealed record Query(
        string UserId);

    public sealed record Response(
        UserInfo UserInfo);

    public sealed record UserInfo(
        string UserId,
        string DisplayName,
        string AvatarUri,
        string Bio,
        Stats Stats);

    public sealed record Stats(
        int LikeCount,
        int FollowerCount,
        int FollowingCount);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        CancellationToken token)
        => await dbContext.Users
        .AsSplitQuery()
        .Include(user => user.Clips)
            .Where(user => user.Id == query.UserId)
            .Select(user => new Response(new UserInfo(
            user.Id,
            user.DisplayName,
            user.AvatarUri,
            user.ProfileBio,
            new Stats(
                user.LikesReceivedCount,
                user.FollowerCount,
                user.FollowingCount))))
            .FirstAsync();
}

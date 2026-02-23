using Clutch.Database;
using Clutch.Database.Entities.Follows;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.Follows.Services;

public sealed class FollowCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetFollow.Request, Follow>> ownedHandler
) : ApplicationCacheBase<GetFollow.Request, Follow>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetFollow.Request request) =>
        string.Create(CultureInfo.InvariantCulture, $"{nameof(Follow)}-{request.InitiatorUserId}:{request.TargetUserId}");
}


[Handler]
public static partial class GetFollow
{
    public sealed record Request
    {
        public required string InitiatorUserId { get; init; }
        public required string TargetUserId { get; init; }
    }

    private static async ValueTask<Follow?> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.Follows
            .Where(follow => follow.InitiatorUserId == request.InitiatorUserId
            && follow.TargetUserId == request.TargetUserId)
            .FirstOrDefaultAsync();
}

public static class FollowCacheExtensions
{
    public static async ValueTask<bool> FollowsUser(this FollowCache cache, GetFollow.Request request)
        => await cache.GetValue(request) is { };
}




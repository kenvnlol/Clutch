using Clutch.Database;
using Clutch.Database.Entities.Likes;
using Clutch.Features.CommentLikes.Shared;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.Likes.Services;

public sealed class LikeCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetLike.Request, Like>> ownedHandler
) : ApplicationCacheBase<GetLike.Request, Like>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetLike.Request request) =>
        string.Create(CultureInfo.InvariantCulture, $"{nameof(Like)}-{request.UserId}:{request.ClipId}");
}


[Handler]
public static partial class GetLike
{
    public sealed record Request
    {
        public required int ClipId { get; init; }
        public required string UserId { get; init; }
    }

    private static async ValueTask<Like?> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.Likes
            .Where(like =>
                like.AuthorId == request.UserId
                && like.ClipId == request.ClipId)
            .FirstOrDefaultAsync();
}

public static class LikeCacheExtensions
{
    public static async ValueTask<bool> HasLikedClip(this LikeCache cache, GetLike.Request request)
    {
        var like = await cache.GetValue(request);

        if (like is null)
        {
            return false;
        }

        return like.LikeType == LikeType.Like;
    }
}




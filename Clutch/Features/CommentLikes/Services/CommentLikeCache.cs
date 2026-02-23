using Clutch.Database;
using Clutch.Database.Entities.CommentLikes;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.CommentLikes.Services;

public sealed class CommentLikeCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetCommentLike.Request, CommentLike>> ownedHandler
) : ApplicationCacheBase<GetCommentLike.Request, CommentLike>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetCommentLike.Request request) =>
        string.Create(CultureInfo.InvariantCulture, $"{nameof(CommentLike)}-{request.UserId}:{request.CommentId}");
}


[Handler]
public static partial class GetCommentLike
{
    public sealed record Request
    {
        public required int CommentId { get; init; }
        public required string UserId { get; init; }
    }

    private static async ValueTask<CommentLike?> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.CommentLikes
            .Where(like => like.InitiatorUserId == request.UserId && like.CommentId == request.CommentId)
            .FirstOrDefaultAsync();
}

public static class CommentLikeCacheExtensions
{
    public static async ValueTask<bool> LikesComment(this CommentLikeCache cache, GetCommentLike.Request request)
        => await cache.GetValue(request) is { };
}
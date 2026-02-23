using Clutch.Database;
using Clutch.Database.Entities.Comments;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using LinqToDB;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.CommentThreads.Services;

public sealed class CommentCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetComment.Request, Comment>> ownedHandler
) : ApplicationCacheBase<GetComment.Request, Comment>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetComment.Request request) =>
        string.Create(CultureInfo.InvariantCulture, $"Comment-{request.CommentId}");
}


[Handler]
public static partial class GetComment
{
    public sealed record Request
    {
        public required long CommentId { get; init; }
    }

    private static async ValueTask<Comment?> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.Comments
            .Where(comment => comment.Id == request.CommentId)
            .FirstOrDefaultAsync();
}

public static class CommentCacheExtensions
{
    public static async ValueTask<string> GetAuthor(this CommentCache cache, GetComment.Request request)
        => (await cache.GetValue(request)).AuthorId;
}
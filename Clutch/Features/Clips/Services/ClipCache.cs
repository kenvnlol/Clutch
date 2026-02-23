using Clutch.Database;
using Clutch.Database.Entities.Clips;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using LinqToDB;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.Clips.Services;

public sealed class ClipCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetClip.Request, Clip>> ownedHandler
) : ApplicationCacheBase<GetClip.Request, Clip>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetClip.Request request)
        => string.Create(CultureInfo.InvariantCulture, $"Clip-{request.ClipId}");
}


[Handler]
public static partial class GetClip
{
    public sealed record Request
    {
        public required int ClipId { get; init; }
    }

    private static async ValueTask<Clip> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.Clips
            .Where(clip => clip.Id == request.ClipId)
            .FirstAsync();
}

public static class ClipCacheExtensions
{
    public static async ValueTask<string> GetAuthor(this ClipCache cache, GetClip.Request request)
        => (await cache.GetValue(request)).AuthorId;
}

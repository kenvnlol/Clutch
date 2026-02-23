using Clutch.Database;
using Clutch.Database.Entities.Saves;
using Immediate.Cache;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Clutch.API.Features.Saves.Services;

public sealed class SaveCache(
    IMemoryCache memoryCache,
    Owned<IHandler<GetSave.Request, Save>> ownedHandler
) : ApplicationCacheBase<GetSave.Request, Save>(
    memoryCache,
    ownedHandler
)
{
    protected override string TransformKey(GetSave.Request request) =>
        string.Create(CultureInfo.InvariantCulture, $"{nameof(Save)}-{request.UserId}:{request.ClipId}");
}


[Handler]
public static partial class GetSave
{
    public sealed record Request
    {
        public required int ClipId { get; init; }
        public required string UserId { get; init; }
    }

    private static async ValueTask<Save?> HandleAsync(
        Request request,
        ApplicationDbContext context,
        CancellationToken token
    ) =>
        await context.Saves
            .Where(s => s.AuthorId == request.UserId
                     && s.ClipId == request.ClipId)
            .FirstOrDefaultAsync(token);
}


public static class SaveCacheExtensions
{
    public static async ValueTask<bool> HasSavedClip(this SaveCache cache, GetSave.Request request)
        => await cache.GetValue(request) is { };

}
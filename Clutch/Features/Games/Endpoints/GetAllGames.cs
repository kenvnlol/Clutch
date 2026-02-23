using Clutch.Database;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Games.Endpoints;

[Handler]
[MapGet("/games")]
public static partial class GetAllGames
{
    public sealed record Query;
    public sealed record GameDto(int Id, string Name);
    private static async ValueTask<IReadOnlyList<GameDto>> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        CancellationToken token)
            => await dbContext.Games.Select(x =>
                new GameDto(x.Id, x.Title)).ToListAsync();
}

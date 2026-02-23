using Clutch.Database;
using Clutch.Features.Clips.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Feeds.Endpoints;

[Handler]
[MapGet("/feed/for-you")]
// TODO: make sure index is usedf here onm view checkup.
public static partial class ForYouFeed
{
    public sealed record Query(
        int Cursor = 0);

    public sealed record Response(
        int? Cursor,
        IReadOnlyList<ClipDto> Clips);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        UserService userService,
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var clips = await dbContext
            .Clips
            .Where(clip => query.Cursor == default || clip.Id < query.Cursor) // Cursor-based paginatio
            .Where(clip => !clip.Views.Select(view => view.AuthorId).Contains(currentUserId))
            .OrderByDescending(clip => clip.Id) // Ensuring proper cursor order
            .Where(clip => clip.AuthorId != userService.GetCurrentUserId())
            .Take(5).Select(clip => new ClipDto(
                clip.Id,
                clip.Likes.Any(like => like.AuthorId == currentUserId),
                clip.Saves.Any(save => save.AuthorId == currentUserId),
                clip.Description,
                clip.Timestamp.ToString(),
                ClipMediaDto.FromClipMedia(clip.Media, configuration),
                new ClipAuthorDto(clip.Author.Id, clip.Author.DisplayName!),
                new ClipStats(
                    clip.ViewCount,
                    clip.CommentCount,
                    clip.ShareCount,
                    clip.LikeCount,
                    clip.SaveCount),
                clip.ExternalVideoUrl)
            ).ToListAsync();

        var nextCursor = clips.LastOrDefault()?.Id ?? null;

        return new Response(nextCursor, clips);
    }
}

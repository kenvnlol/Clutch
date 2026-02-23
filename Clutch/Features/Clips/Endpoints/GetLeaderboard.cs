using Clutch.Database;
using Clutch.Database.Entities.Clips.Extensions;
using Clutch.Features.Clips.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Clips.Endpoints;

[Handler]
[MapGet("/clips/weekly-leaderboard")]

public static partial class GetLeaderboard
{
    public sealed record Query(
        int LikeCursor = int.MaxValue,
        int IdCursor = int.MaxValue);

    public sealed record Response(
        int LikeCursor,
        int IdCursor,
        IReadOnlyList<ClipDto> Clips);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        UserService userService,
        ApplicationDbContext dbContext,
        IConfiguration configuration,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var clips = await dbContext.Clips
            .GetInCurrentContest()
            .Where(clip =>
                clip.LikeCount < query.LikeCursor ||
                (clip.LikeCount == query.LikeCursor && clip.Id < query.IdCursor))
            .OrderByDescending(clip => clip.LikeCount)
            .ThenByDescending(clip => clip.Id)
            .Take(5)
            .Select(clip => new ClipDto(
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
                clip.ExternalVideoUrl))
            .ToListAsync(token);

        return new Response(
            clips.LastOrDefault()?.Stats.LikeCount ?? 0,
            clips.LastOrDefault()?.Id ?? 0,
            Clips: clips
        );
    }
}

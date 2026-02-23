using Clutch.Database;
using Clutch.Features.Clips.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Saves.Endpoints;

[Handler]
[MapGet("/user/collections/saved")]
public static partial class GetSavedClips
{
    public sealed record Query(
        int Cursor = 0);
    public record Response(
        int? Cursor,
        IReadOnlyList<ClipDto> Clips);

    public record ClipDto(
    int Id,
    bool Liked,
    DateTimeOffset SavedAt,
    string Description,
    string UploadDate,
    ClipMediaDto Media,
    UserDto Author,
    Stats Stats);

    public sealed record Stats(
        int ViewCount,
        int CommentCount,
        int ShareCount,
        int LikeCount,
        int SaveCount);

    public record UserDto(
        string UserId,
        string DisplayName);

    private static async ValueTask<Response> HandleAsync(
       Query query,
       ApplicationDbContext dbContext,
       UserService userService,
       IConfiguration configuration,
       CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var clips = await dbContext.Likes
            .Where(like => like.AuthorId == currentUserId)
            .Where(m => query.Cursor == default || m.Id < query.Cursor)
            .OrderByDescending(m => m.Id)
            .Take(20)
            .Select(like => new ClipDto(
                like.Clip.Id,
                dbContext.Saves.Any(like => like.AuthorId == currentUserId && like.ClipId == like.ClipId),
                like.Timestamp,
                like.Clip.Description,
                like.Clip.Timestamp.ToString(),
                ClipMediaDto.FromClipMedia(like.Clip.Media, configuration),
                new UserDto(
                    like.Clip.AuthorId,
                    like.Clip.Author.DisplayName
                ),
                new Stats(
                    like.Clip.ViewCount,
                    like.Clip.CommentCount,
                    like.Clip.ShareCount,
                    like.Clip.LikeCount,
                    like.Clip.SaveCount
                )
            )).ToListAsync(token);

        var nextCursor = clips.LastOrDefault()?.Id;

        return new Response(nextCursor, clips);
    }
}

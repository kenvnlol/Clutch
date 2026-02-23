using Clutch.Database;
using Clutch.Features.Clips.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Likes.Endpoints;

[Handler]
[MapGet("/user/collections/liked")]
// TODO: the dbcontext call inside the select loooks suspicious. 
public static partial class GetLikedClips
{
    public sealed record Query(
        string UserId,
        int Cursor = 0);

    public record Response(
        int? Cursor,
        IReadOnlyList<ClipDto> Clips);

    public record ClipDto(
    int Id,
    DateTimeOffset LikedAt,
    bool Saved,
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
                like.Timestamp,
                dbContext.Saves.Any(like => like.AuthorId == currentUserId && like.ClipId == like.ClipId),
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

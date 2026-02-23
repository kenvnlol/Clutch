using Clutch.Database;
using Clutch.Features.Clips.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.Clips.Endpoints;

[Handler]
[MapGet("/user/followed/feed")]
// TODO fix the side feed
public static partial class SideFeed
{
    public sealed record Query(int Cursor = 0);

    public sealed record Response(int? Cursor, IReadOnlyList<ClipDto> Clips);

    private static async ValueTask HandleAsync(
        Query query,
        UserService userService,
        ApplicationDbContext dbContext,
        CancellationToken token)
    {
        //var currentUserId = userService.GetCurrentUserId();

        //// Fetch clips from followed users with cursor-based pagination.
        //var clips = await dbContext
        //    .Clips
        //    .Where(clip => followedUserIds.Contains(clip.AuthorId))
        //    .Where(clip => query.Cursor == default || clip.Id < query.Cursor)
        //    .OrderByDescending(clip => clip.Id)
        //    .Take(5)
        //    .Select(clip => new ClipDto(
        //        clip.Id,
        //        clip.Likes.Any(like => like.AuthorId == currentUserId),
        //        clip.Saves.Any(save => save.AuthorId == currentUserId),
        //        clip.Description,
        //        clip.Timestamp.ToString(),
        //        clip.VideoBlobUri,
        //        clip.AvatarBlobUri,
        //        new ClipAuthorDto(clip.Author.Id, clip.Author.DisplayName!),
        //        new ClipStats(
        //            clip.ViewCount,
        //            clip.CommentCount,
        //            clip.ShareCount,
        //            clip.LikeCount,
        //            clip.SaveCount)))
        //    .ToListAsync(token);

        //// Determine the next cursor based on the last clip.
        //var nextCursor = clips.LastOrDefault()?.Id;

        //return new Response(nextCursor, clips);

        return;
    }
}
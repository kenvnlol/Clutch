using Clutch.API.Features.Likes.Services;
using Clutch.Database;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.Likes.Endpoints;


[Handler]
[MapPost("/clip/like")]
// TODO: Here, we likely want to get the clip author from a cache to avoid a DB call.
public static partial class CreateOrRemoveLike
{
    public sealed record Command(
        int ClipId,
        LikeType LikeType,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform);

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        LikeCache cache,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var like = new LikeEventData(
                command.ClipId,
                command.DeviceId,
                currentUserId,
                command.Platform,
                command.BrowserLanguage,
                command.DevicePlatform,
                true,
                DateTimeOffset.UtcNow,
                command.LikeType);

        var userEvent = new UserEvent
        {
            EntityName = nameof(Like),
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = command.LikeType == LikeType.Like ? EventActionType.Create : EventActionType.Delete,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            LikeEvent = like,
            UserIsLoggedIn = true
        };

        dbContext.UserEvents.Add(userEvent);
        await dbContext.SaveChangesAsync();

        // Optimistic caching.
        cache.SetValue(
            new GetLike.Request
            {
                UserId = currentUserId,
                ClipId = command.ClipId
            },
            like.ToEntity(userEvent.Id, string.Empty));
    }
}


using Clutch.API.Features.CommentLikes.Services;
using Clutch.Database;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.CommentLikes.Shared;
using Clutch.Features.Users.Services;
using IdGen;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.CommentLikes;


[Handler]
[MapPost("/clip/comment-thread/comment/like")]
// TODO: Here, we either want cache on inboxRecipient or "indexing". 
public static partial class CreateOrRemoveCommentLike
{
    public sealed record Command(
        int CommentId,
        LikeType LikeType,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform);

    private static async ValueTask HandleAsync(
        Command command, 
        ApplicationDbContext dbContext,
        UserService userService,
        CommentLikeCache cache,
        IdGenerator idGen,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var commentLikeId = idGen.CreateId();

        var commentLike = new CommentLikeEventData(
                        command.CommentId,
                        currentUserId,
                        DateTimeOffset.UtcNow,
                        command.LikeType);

        var userEvent = new UserEvent
        {
            EntityName = nameof(CommentLike),
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = command.LikeType == LikeType.Like ? EventActionType.Create : EventActionType.Delete,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            CommentLikeEvent = commentLike,
            UserIsLoggedIn = true
        };

        dbContext.UserEvents.Add(userEvent);
        await dbContext.SaveChangesAsync();

        cache.SetValue(
            new GetCommentLike.Request
            {
                CommentId = command.CommentId,
                UserId = currentUserId
            },
            commentLike.ToEntity(userEvent.Id, string.Empty));
    }
}

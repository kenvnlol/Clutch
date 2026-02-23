using Clutch.API.Features.Follows.Services;
using Clutch.Database;
using Clutch.Database.Entities.CommentLikes;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.Follows.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.Follows.Endpoints;


[Handler]
[MapPost("/user/follow")]
// TODO: DestinationUri(?) 
public static partial class CreateOrRemoveFollow
{
    public sealed record Command(
        string RecipientUserId,
        FollowType FollowType,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform,
        string DeviceId);

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        FollowCache cache,
        IUserEventWriter userEventWriter,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var utcNow = DateTimeOffset.UtcNow;

        var follow = new FollowEventData(
                currentUserId,
                command.RecipientUserId,
                utcNow,
                command.FollowType);

        var userEvent = new UserEvent
        {
            EntityName = nameof(CommentLike),
            Timestamp = utcNow,
            ActionType = command.FollowType == FollowType.Follow ? EventActionType.Create : EventActionType.Delete,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            FollowEvent = follow,
            UserIsLoggedIn = true
        };

        await userEventWriter.AppendAsync(userEvent, token);

        cache.SetValue(
            new GetFollow.Request
            {
                InitiatorUserId = currentUserId,
                TargetUserId = command.RecipientUserId
            },
            follow.ToEntity(userEvent.Id));
    }
}


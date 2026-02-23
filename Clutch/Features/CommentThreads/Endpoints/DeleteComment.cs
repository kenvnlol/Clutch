using Clutch.API.Features.CommentThreads.Services;
using Clutch.Database;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.CommentThreads.Endpoints;

[Handler]
[MapDelete("/clip/comment-thread/comment")]
public static partial class DeleteComment
{
    public sealed record Command(
        long Id,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform,
        string DeviceId);
    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        CommentCache cache,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var utcNow = DateTimeOffset.UtcNow;

        var comment = new CommentEventData(
              CommentId: command.Id,
              CommentThreadId: (int)default,
              RootCommentId: (int)default,
              ParentCommentId: (int)default,
              AuthorId: currentUserId,
              Text: string.Empty,
              DeviceId: command.DeviceId,
              Platform: command.Platform,
              BrowserLanguage: command.BrowserLanguage,
              DevicePlatform: command.DevicePlatform,
              UserIsLoggedIn: true,
              Timestamp: utcNow,
              Type: CommentType.Delete
          );

        var userEvent = new UserEvent
        {
            EntityName = nameof(Comment),
            Timestamp = utcNow,
            ActionType = EventActionType.Create,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            UserIsLoggedIn = true,
            CommentEvent = comment
        };

        dbContext.UserEvents.Add(userEvent);
        await dbContext.SaveChangesAsync();

        cache.SetValue(
            new GetComment.Request { CommentId = command.Id },
            comment.ToEntity(userEvent.Id, string.Empty));
    }
}




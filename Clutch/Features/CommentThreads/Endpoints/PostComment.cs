using Clutch.API.Features.CommentThreads.Services;
using Clutch.Database;
using Clutch.Database.Entities.Comments;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.Users.Services;
using IdGen;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.API.Features.CommentThreads.Endpoints;

[Handler]
[MapPost("clip/comment-thread/comment")]
public static partial class PostComment
{
    public sealed record Command(
        int ClipId,
        string Text,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform,
        int ParentCommentId = 0,
        int RootCommentId = 0);

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        IdGenerator idGen,
        CommentCache cache,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var utcNow = DateTimeOffset.UtcNow;
        var commentId = idGen.CreateId();


        var comment = new CommentEventData(
                CommentId: commentId,
                CommentThreadId: command.ClipId,
                RootCommentId: command.RootCommentId == 0 ? null : command.RootCommentId,
                ParentCommentId: command.ParentCommentId == 0 ? null : command.ParentCommentId,
                AuthorId: currentUserId,
                Text: command.Text,
                DeviceId: command.DeviceId,
                Platform: command.Platform,
                BrowserLanguage: command.BrowserLanguage,
                DevicePlatform: command.DevicePlatform,
                UserIsLoggedIn: true,
                Timestamp: utcNow,
                CommentType.Delete
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
            new GetComment.Request { CommentId = commentId },
            comment.ToEntity(userEvent.Id, string.Empty));
    }
}


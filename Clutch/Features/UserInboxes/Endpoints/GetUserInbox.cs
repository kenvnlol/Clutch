using Clutch.Database;
using Clutch.Database.Entities.InboxActivities;
using Clutch.Database.Entities.UserInboxes;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Users.Endpoints;

[Handler]
[MapGet("/user/inbox")]
public static partial class GetUserInbox
{
    public sealed record Query(
        int Cursor = 0);

    public sealed record Response(
        List<InboxItem> Items,
        int? Cursor);

    public sealed record InboxItem(
        int Id,
        DateTimeOffset? ReadAt,
        InboxNotificationType Type,
        DateTimeOffset Timestamp,
        string Message,
        string? ResourceAvatar,
        string? Destination,
        UserDetails Initiator);

    public sealed record UserDetails(
        string Id,
        string DisplayName,
        string AvatarUri);

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var userInbox = await dbContext.UserInboxes
          .AsSplitQuery()
          .Include(inbox => inbox.Activities)
          .ThenInclude(activity => activity.Initiator)
          .Where(inbox => inbox.UserId == currentUserId)
          .SelectMany(inbox => inbox.Activities)
          .Where(activity => query.Cursor == 0 || activity.Id < query.Cursor)
          .OrderByDescending(activity => activity.Id)
          .Take(20)
          .ToListAsync(token);

        var inboxItems = userInbox.Select(activity => new InboxItem(
                 activity.Id,
                 activity.ReadAt,
                 activity.Type,
                 activity.Timestamp,
                 activity.GetDisplayMessage(),
                 activity.GetResourceAvatar(),
                 activity.DestinationUri,
                 new UserDetails(
                     activity.Initiator.Id,
                     activity.Initiator.DisplayName,
                     activity.Initiator.AvatarUri)
             )).ToList();

        var nextCursor = inboxItems.LastOrDefault()?.Id;

        return new Response(inboxItems, nextCursor);
    }
}

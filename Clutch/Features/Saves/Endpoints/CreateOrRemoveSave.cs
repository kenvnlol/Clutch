using Clutch.API.Features.Saves.Services;
using Clutch.Database;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Saves.Endpoints;


[Handler]
[MapPost("/clip/save")]
public static partial class CreateOrRemoveSave
{
    public sealed record Command(
        int ClipId,
        SaveType SaveType,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform);

    private static async ValueTask HandleAsync(
        Command command,
        UserService userService,
        ApplicationDbContext dbContext,
        SaveCache cache,
        IConfiguration configuration,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();

        var save = new SaveEventData(
                command.ClipId,
                command.DeviceId,
                currentUserId,
                command.Platform,
                command.BrowserLanguage,
                command.DevicePlatform,
                true,
                DateTimeOffset.UtcNow,
                command.SaveType
            );

        var userEvent = new UserEvent
        {
            EntityName = nameof(Save),
            Timestamp = DateTimeOffset.UtcNow,
            ActionType = command.SaveType == SaveType.Save
                             ? EventActionType.Create
                             : EventActionType.Delete,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            SaveEvent = save,
            UserIsLoggedIn = true
        };

        dbContext.UserEvents.Add(userEvent);
        await dbContext.SaveChangesAsync();

        cache.SetValue(
            new GetSave.Request
            {
                UserId = currentUserId,
                ClipId = command.ClipId
            },
            save.ToEntity(userEvent.Id));
    }
}
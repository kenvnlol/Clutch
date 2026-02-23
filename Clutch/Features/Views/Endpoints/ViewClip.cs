using Clutch.Database;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.Views;
using Clutch.Features.Users.Services;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Immediate.Validations.Shared;

namespace Clutch.API.Features.Clips.Endpoints;


[Handler]
[MapPost("/clip/view")]
public static partial class CreateViewEvent
{
    [Validate]
    public partial record Command(
        int ClipId,
        DateTimeOffset StartedAt,
        DateTimeOffset EndedAt,
        string DeviceId,
        string Platform,
        string BrowserLanguage,
        string DevicePlatform
    ) : IValidationTarget<Command>
    {
        private static void AdditionalValidations(
            ValidationResult errors,
            Command target)
        {
            if (target.EndedAt <= target.StartedAt)
            {
                errors.Add(new ValidationError
                {
                    PropertyName = nameof(EndedAt),
                    ErrorMessage = $"'{nameof(EndedAt)}' must be after '{nameof(StartedAt)}'."
                });
                return;
            }

            var secondsWatched = (target.EndedAt - target.StartedAt).TotalSeconds;
            if (secondsWatched < 2)
            {
                errors.Add(new ValidationError
                {
                    PropertyName = nameof(EndedAt),
                    ErrorMessage = $"Watch time must be at least 2 seconds, but was {secondsWatched:F1}."
                });
            }
        }
    }

    private static async ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        CancellationToken token)
    {
        // who’s watching?
        var currentUserId = userService.IsLoggedIn()
            ? userService.GetCurrentUserId()
            : null;

        var currentUtc = DateTimeOffset.UtcNow;

        var view = new ViewEventData(
                ClipId: command.ClipId,
                AuthorId: null, // we’re not loading author here
                ViewDurationInSeconds: (command.EndedAt - command.StartedAt).TotalSeconds,
                PercentViewed: 0m,
                Muted: false,
                ReplayCount: 0,
                DeviceId: command.DeviceId,
                Platform: command.Platform,
                BrowserLanguage: command.BrowserLanguage,
                DevicePlatform: command.DevicePlatform,
                UserIsLoggedIn: currentUserId is not null,
                Timestamp: currentUtc);

        var userEvent = new UserEvent
        {
            EntityName = nameof(View),
            Timestamp = currentUtc,
            ActionType = EventActionType.Create,
            Platform = command.Platform,
            BrowserLanguage = command.BrowserLanguage,
            DevicePlatform = command.DevicePlatform,
            DeviceId = command.DeviceId,
            ViewEvent = view,
            UserIsLoggedIn = currentUserId is { }
        };

        dbContext.UserEvents.Add(userEvent);
        await dbContext.SaveChangesAsync(token);
    }
}


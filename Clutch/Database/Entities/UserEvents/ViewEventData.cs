using Clutch.Database.Entities.Views;
using Clutch.Features.Shared.Interfaces;

public record ViewEventData(
    int ClipId,
    string? AuthorId,
    double ViewDurationInSeconds,
    decimal PercentViewed,
    bool Muted,
    int ReplayCount,
    string DeviceId,
    string Platform,
    string BrowserLanguage,
    string DevicePlatform,
    bool UserIsLoggedIn,
    DateTimeOffset Timestamp
) : ICompactionKeyProvider
{
    public View ToEntity(int eventId) => new View
    {
        EventId = eventId,
        ClipId = ClipId,
        AuthorId = AuthorId,
        ViewDurationInSeconds = ViewDurationInSeconds,
        PercentViewed = PercentViewed,
        Muted = Muted,
        ReplayCount = ReplayCount,
        DeviceId = DeviceId,
        Platform = Platform,
        BrowserLanguage = BrowserLanguage,
        DevicePlatform = DevicePlatform,
        UserIsLoggedIn = UserIsLoggedIn,
        Timestamp = Timestamp,
    };
}


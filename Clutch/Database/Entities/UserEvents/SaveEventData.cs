
using Clutch.Database.Entities.Saves;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Shared.Interfaces;

public record SaveEventData(
    int ClipId,
    string AuthorId,
    string DeviceId,
    string Platform,
    string BrowserLanguage,
    string DevicePlatform,
    bool UserIsLoggedIn,
    DateTimeOffset Timestamp,
    SaveType Type
) : ICompactionKeyProvider
{
    public Save ToEntity(int eventId) => new Save
    {
        Timestamp = Timestamp,
        DeviceId = DeviceId,
        Platform = Platform,
        AuthorId = AuthorId,
        ClipId = ClipId,
        DevicePlatform = DevicePlatform,
        BrowserLanguage = BrowserLanguage,
        UserIsLoggedIn = true,
        EventId = eventId,
        SaveType = Type
    };
}



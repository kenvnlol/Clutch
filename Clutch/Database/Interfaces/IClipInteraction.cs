using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.Users;
using Clutch.Database.Interceptors;

public interface IClipInteraction : ISoftDeletable, IDenormalizedStat, IUserEvent
{
    public string DeviceId { get; }
    public string AuthorId { get; }
    public User Author { get; }
    public Clip Clip { get; }
    public int ClipId { get; }
    public string Platform { get; }
    public string BrowserLanguage { get; }
    public string DevicePlatform { get; }
    public bool UserIsLoggedIn { get; }
    public DateTimeOffset Timestamp { get; }
}

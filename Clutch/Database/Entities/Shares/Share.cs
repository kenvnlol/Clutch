using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.Shares;

public class Share : IClipInteraction
{
    public int Id { get; set; }
    public required string DeviceId { get; init; }
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public Clip Clip { get; init; } = null!;
    public int ClipId { get; init; }
    public required string Platform { get; init; }
    public required string BrowserLanguage { get; init; }
    public required string DevicePlatform { get; init; }
    public bool UserIsLoggedIn { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public required ShareDestination Destination { get; init; }
    public bool IsDeleted { get; set; }

    public required int EventId { get; init; }
}

using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Users;
using Clutch.Features.Saves.Shared;
using Clutch.Features.Shared.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.Saves;

public class Save : IClipInteraction, ICompactionKeyProvider
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
    public bool IsDeleted { get; set; }

    public required int EventId { get; init; }

    /// <summary>
    /// In-memory toggle state for cache/materializer:
    /// true = user saved; false = user unsaved (tombstone).
    /// Not persisted to the database.
    /// </summary>
    [NotMapped]
    public SaveType SaveType { get; init; }
}
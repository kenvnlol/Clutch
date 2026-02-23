using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Users;
using Clutch.Features.Shared.Interfaces;
using System.Diagnostics.CodeAnalysis;

namespace Clutch.Database.Entities.Views;
public class View : IClipInteraction, ICompactionKeyProvider
{
    public int Id { get; set; }
    public double ViewDurationInSeconds { get; set; }
    public decimal PercentViewed { get; set; }
    public bool Muted { get; set; }
    public int ReplayCount { get; set; }
    public required string DeviceId { get; init; }
    [MemberNotNullWhen(true, nameof(UserIsLoggedIn))]
    public User? Author { get; init; }
    [MemberNotNullWhen(true, nameof(UserIsLoggedIn))]
    public string? AuthorId { get; init; }
    public Clip Clip { get; init; } = null!;
    public int ClipId { get; init; }
    public required string Platform { get; init; }
    public required string BrowserLanguage { get; init; }
    public required string DevicePlatform { get; init; }
    public bool UserIsLoggedIn { get; init; }
    public required DateTimeOffset Timestamp { get; init; }
    public bool IsDeleted { get; set; }

    public required int EventId { get; init; }
    //public CounterAggregationContext? CountedContext { get; set; }
}


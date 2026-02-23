using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Games;
using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.StagingClips;

public class StagingClip
{
    public int Id { get; init; }
    public required BlobReference BlobReference { get; init; }
    public Game Game { get; init; } = null!;
    public int GameId { get; init; }
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public required string Description { get; init; }
    public required DateTimeOffset Timestamp { get; set; }
}

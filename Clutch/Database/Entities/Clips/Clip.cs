using Clutch.Database.Entities.CommentThreads;
using Clutch.Database.Entities.ContestWinners;
using Clutch.Database.Entities.Games;
using Clutch.Database.Entities.Likes;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.Shares;
using Clutch.Database.Entities.Users;
using Clutch.Database.Entities.Views;

namespace Clutch.Database.Entities.Clips;

public class Clip
{
    public int Id { get; set; }
    public Game Game { get; init; } = null!;
    public int GameId { get; init; }
    public required CommentThread CommentThread { get; set; }
    public required string AuthorId { get; init; }
    public User Author { get; init; } = null!;
    public required ClipMedia Media { get; init; }
    public required string Description { get; init; }
    public string? ExternalVideoUrl { get; set; }
    public int DurationInSeconds { get; set; }
    public int CommentCount { get; set; } = 0;
    public int ShareCount { get; set; } = 0;
    public int LikeCount { get; set; } = 0;
    public int ViewCount { get; set; } = 0;
    public int SaveCount { get; set; } = 0;
    public required DateTimeOffset Timestamp { get; set; }
    public DateTimeOffset LastCounterUpdate { get; set; }
    public required List<Like> Likes { get; init; }
    public required List<View> Views { get; init; }
    public required List<Share> Shares { get; init; }
    public required List<Save> Saves { get; init; }
    public required List<ContestWinner> ContestWins { get; init; }
}



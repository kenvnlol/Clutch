using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Contests;

namespace Clutch.Database.Entities.ContestWinners;

public sealed class ContestWinner
{
    public int Id { get; init; }
    public int ClipId { get; init; }
    public Clip Clip { get; init; } = null!;
    public int ContestId { get; init; }
    public Contest Contest { get; init; } = null!;
    public int Placement { get; init; }
    public decimal PrizeAmount { get; init; }
}
using Clutch.Database.Entities.ContestWinners;

namespace Clutch.Database.Entities.Contests;

public sealed class Contest
{
    public int Id { get; set; }
    public required DateTimeOffset Start { get; init; }
    public required DateTimeOffset End { get; init; }
    public required List<ContestWinner> Winners { get; init; }
    public required int SubmissionCount { get; init; }
    public required int TotalLikes { get; init; }
    public required int TotalViews { get; init; }
    public required HashSet<string> Sponsors { get; init; }
}
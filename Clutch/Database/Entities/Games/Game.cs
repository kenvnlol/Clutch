using Clutch.Database.Entities.Clips;

namespace Clutch.Database.Entities.Games;

public class Game
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public required List<Clip> Clips { get; init; }
}
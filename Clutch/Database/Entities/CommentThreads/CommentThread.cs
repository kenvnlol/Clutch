using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.Comments;

namespace Clutch.Database.Entities.CommentThreads;

public class CommentThread
{
    public int ClipId { get; init; }
    public Clip Clip { get; init; } = null!;
    public required List<Comment> Comments { get; init; }
}
using Clutch.Database;
using Clutch.Features.Shared.Services;

namespace Clutch.Features.CommentLikes.Shared;

public sealed class CommentLikedHandler(ApplicationDbContext dbContext) : UserInteractionEventHandler<CommentLiked>(dbContext)
{
    public override Task PushToInbox(CommentLiked notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateCache(CommentLiked notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateInboxCounters(CommentLiked notification)
    {
        return Task.CompletedTask;
    }

}
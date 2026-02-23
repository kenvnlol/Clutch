using Clutch.Database;
using Clutch.Features.Shared.Services;

namespace Clutch.Features.Likes.Shared;

public class ClipLikedHandler(ApplicationDbContext dbContext) : UserInteractionEventHandler<ClipLiked>(dbContext)
{
    public override Task PushToInbox(ClipLiked notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateCache(ClipLiked notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateInboxCounters(ClipLiked notification)
    {
        return Task.CompletedTask;
    }
}
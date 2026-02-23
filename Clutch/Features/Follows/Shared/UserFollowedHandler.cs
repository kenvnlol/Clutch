using Clutch.Database;
using Clutch.Features.Shared.Services;

namespace Clutch.Features.Follows.Shared;

public class UserFollowedHandler(ApplicationDbContext dbContext) : UserInteractionEventHandler<UserFollowed>(dbContext)
{
    public override Task PushToInbox(UserFollowed notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateCache(UserFollowed notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateInboxCounters(UserFollowed notification)
    {
        return Task.CompletedTask;
    }
}
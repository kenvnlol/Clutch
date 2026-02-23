using Clutch.Database;
using Clutch.Features.Shared.Services;

namespace Clutch.Features.CommentThreads.Shared;

public class CommentPostedHandler(ApplicationDbContext dbContext) : UserInteractionEventHandler<CommentChanged>(dbContext)
{
    public override Task PushToInbox(CommentChanged notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateCache(CommentChanged notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateInboxCounters(CommentChanged notification)
    {
        return Task.CompletedTask;
    }

}
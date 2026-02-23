using Clutch.Database;
using Clutch.Features.Shared.Services;

namespace Clutch.Features.DirectThreads.Shared;

public class MessagePostedHandler(ApplicationDbContext dbContext) : UserInteractionEventHandler<MessagePosted>(dbContext)
{
    public override Task PushToInbox(MessagePosted notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateCache(MessagePosted notification)
    {
        return Task.CompletedTask;
    }

    public override Task UpdateInboxCounters(MessagePosted notification)
    {
        return Task.CompletedTask;
    }

}
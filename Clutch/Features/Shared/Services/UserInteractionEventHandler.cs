using Clutch.Database;
using MediatR;

namespace Clutch.Features.Shared.Services;

public abstract class UserInteractionEventHandler<T>(ApplicationDbContext dbContext) : INotificationHandler<T>
    where T : INotification
{
    public async Task Handle(T notification, CancellationToken cancellationToken)
    {
        await UpdateCache(notification);
        await PushToInbox(notification);
        await UpdateInboxCounters(notification);
    }

    public abstract Task PushToInbox(T notification);

    public abstract Task UpdateCache(T notification);

    public abstract Task UpdateInboxCounters(T notification);
}
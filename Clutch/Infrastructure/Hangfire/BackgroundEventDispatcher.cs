using Hangfire;
using MediatR;

namespace Clutch.Infrastructure.Hangfire;

public class BackgroundEventDispatcher(IBackgroundJobClient hangfire, IMediator mediator)
{
    public void Enqueue<T>(T notification, CancellationToken token)
        where T : INotification
            => hangfire.Enqueue(() => mediator.Publish(notification, token));
}

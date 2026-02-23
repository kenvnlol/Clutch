namespace Clutch.Infrastructure.Jobs;


public interface IRecurringJob
{
    Task Execute(CancellationToken cancellationToken);
}

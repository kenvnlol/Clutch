using Clutch.Infrastructure.Jobs;
using Hangfire;

namespace Clutch.Infrastructure.Hangfire;

public static class JobRegistrationService
{
    public static void Register()
    {
        RecurringJob.AddOrUpdate<EntityMaterializerJob>(
            nameof(EntityMaterializerJob),
            job => job.Execute(CancellationToken.None),
            CronExpressions.EveryFiveSeconds
        );

        RecurringJob.AddOrUpdate<CounterBatchJob>(
            nameof(CounterBatchJob),
            job => job.Execute(CancellationToken.None),
            CronExpressions.EveryFiveSeconds
        );

        //Finalize weekly contest every Sunday at 10:00 UTC
        RecurringJob.AddOrUpdate<FinalizeWeeklyContestJob>(
            nameof(FinalizeWeeklyContestJob),
            job => job.Execute(CancellationToken.None),
            "0 10 * * 0" // At 10:00 on Sunday UTC
        );
    }
}

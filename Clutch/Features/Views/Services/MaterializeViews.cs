using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.UserEvents;
using Clutch.Database.Entities.Views;
using Clutch.Features.Shared.Extensions;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.Views.Services;

[Handler]
public sealed partial class MaterializeViews(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset => offset.ConsumerGroup == EventConsumerGroup.ViewEntity);

        if (lastOffset == null)
        {
            return;
        }

        var compactedViews = await dbContext.UserEvents.FetchAndCompactFromOffsetAsync<ViewEventData>(lastOffset);

        var existingSaves = await GetExistingAsync(compactedViews, dbContext, cancellationToken);

        Materialize(compactedViews, existingSaves, dbContext);

        lastOffset.AdvanceTo(compactedViews);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<View>> GetExistingAsync(
        List<CompactionResult<ViewEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.Views
            .Where(v => keys.Any(k => k == ((v.AuthorId ?? v.DeviceId) + ":" + v.ClipId)))
            .AsTracking()
            .ToListAsync();

        return query;
    }

    private static void Materialize(
    List<CompactionResult<ViewEventData>> compactedViews,
    List<View> existingViews,
    ApplicationDbContext dbContext)
    {
        foreach (var ev in compactedViews)
        {
            var match = existingViews
                .FirstOrDefault(v => v.GetCompactionKey() == ev.Data.GetCompactionKey());

            switch (match)
            {
                case null:
                    {
                        var entity = ev.Data.ToEntity(ev.EventId);
                        entity.ReplayCount = ev.TotalCount - 1;
                        dbContext.Views.Add(entity);
                        break;
                    }

                default:
                    match.ReplayCount += ev.TotalCount;
                    break;
            }
        }
    }
}

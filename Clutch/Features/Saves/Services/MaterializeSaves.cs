using Clutch.Database;
using Clutch.Database.Entities;
using Clutch.Database.Entities.Saves;
using Clutch.Database.Entities.UserEvents;
using Clutch.Features.Shared.Extensions;
using CommunityToolkit.Diagnostics;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.Features.Saves.Services;

[Handler]
public sealed partial class MaterializeSaves(ApplicationDbContext dbContext)
{
    public sealed record Request;
    private async ValueTask HandleAsync(
        Request _,
        CancellationToken cancellationToken)
    {
        // put these offsets into their own tables, actually. with one item in eac. to avoid deadlocks. 
        var lastOffset = await dbContext.EventConsumerOffsets
            .AsTracking()
            .FirstOrDefaultAsync(offset =>
                offset.ConsumerGroup == EventConsumerGroup.SaveEntity);


        if (lastOffset == null)
        {
            return;
        }

        var compactedSaves = await dbContext.UserEvents
            .FetchAndCompactFromOffsetAsync<SaveEventData>(lastOffset);

        var existingSaves = await GetExistingAsync(
            compactedSaves,
            dbContext,
            cancellationToken);

        Materialize(
            compactedSaves,
            existingSaves,
            dbContext);

        lastOffset.AdvanceTo(
            compactedSaves);

        await dbContext.SaveChangesAsync();
    }

    public static Task<List<Save>> GetExistingAsync(
        List<CompactionResult<SaveEventData>> compactedBatch,
        ApplicationDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var keys = compactedBatch.Select(c => c.Data.GetCompactionKey()).ToHashSet();

        var query = dbContext.Saves
            .Where(s => keys.Any(k => k == (s.AuthorId + ":" + s.ClipId)))
            .ToListAsync();

        return query;
    }

    private static void Materialize(
    List<CompactionResult<SaveEventData>> compactedSaves,
    List<Save> existingSaves,
    ApplicationDbContext dbContext)
    {
        foreach (var ev in compactedSaves)
        {
            var result = ev.GetEventActionDetails(existingSaves);

            switch (result.Action)
            {
                case EntityAction.Add:
                    dbContext.Saves.Add(ev.Data.ToEntity(ev.EventId));
                    break;

                case EntityAction.Delete:
                    Guard.IsNotNull(result.ExistingMatch);
                    dbContext.Saves.Remove(result.ExistingMatch);
                    break;

                case EntityAction.Skip:
                    break;

                default:
                    break;
            }
        }
    }
}

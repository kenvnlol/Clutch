using Clutch.Features.Shared.Extensions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clutch.Database.Entities.UserEvents;

public static class UserEventExtensions
{
    public static async Task InsertAsync(this UserEvent evt, SqlConnection conn)
    {
        var cols = new List<string>();
        var paramNames = new List<string>();
        var sqlParams = new List<SqlParameter>();
        int idx = 0;

        var topProps = typeof(UserEvent)
            .GetProperties()
            .Where(p => !p.Name.EndsWith("Event", StringComparison.Ordinal));

        foreach (var prop in topProps)
        {
            var col = prop.Name;
            var value = prop.GetValue(evt) ?? DBNull.Value;

            cols.Add(col);
            var pn = "@p" + idx++;
            paramNames.Add(pn);
            sqlParams.Add(new SqlParameter(pn, value));
        }

        var subEventProp = typeof(UserEvent)
            .GetProperties()
            .FirstOrDefault(p => p.Name.EndsWith("Event", StringComparison.Ordinal));

        if (subEventProp != null)
        {
            var subVal = subEventProp.GetValue(evt);
            var prefix = subEventProp.Name;
            foreach (var nested in subEventProp.PropertyType.GetProperties())
            {
                var col = $"{prefix}_{nested.Name}";
                var value = nested.GetValue(subVal) ?? DBNull.Value;

                cols.Add(col);
                var pn = "@p" + idx++;
                paramNames.Add(pn);
                sqlParams.Add(new SqlParameter(pn, value));
            }
        }

        var sql = $@"
                INSERT INTO UserEvents
                  ({string.Join(", ", cols)})
                VALUES
                  ({string.Join(", ", paramNames)})
                ";

        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddRange(sqlParams.ToArray());
        await cmd.ExecuteNonQueryAsync();
    }

    public static async Task<List<CompactionResult<T>>> FetchAndCompactFromOffsetAsync<T>(
       this IQueryable<UserEvent> query,
       EventConsumerOffset offset)
       where T : class
    {
        var (discriminator, selector) = typeof(T) switch
        {
            Type t when t == typeof(LikeEventData) => (
                ue => ue.LikeEvent != null,
                ue =>
                {
                    EnsureGroup(EventConsumerGroup.LikeEntity, offset.ConsumerGroup);
                    return ue.LikeEvent as T;
                }
            ),

            Type t when t == typeof(CommentEventData) => (
                ue => ue.CommentEvent != null,
                ue =>
                {
                    EnsureGroup(EventConsumerGroup.CommentEntity, offset.ConsumerGroup);
                    return ue.CommentEvent as T;
                }
            ),

            Type t when t == typeof(FollowEventData) => (
                ue => ue.FollowEvent != null,
                ue =>
                {
                    EnsureGroup(EventConsumerGroup.FollowEntity, offset.ConsumerGroup);
                    return ue.FollowEvent as T;
                }
            ),
            Type t when t == typeof(CommentLikeEventData) => (
                ue => ue.CommentLikeEvent != null,
                ue =>
                {
                    EnsureGroup(EventConsumerGroup.CommentLikeEntity, offset.ConsumerGroup);
                    return ue.CommentLikeEvent as T;
                }
            ),

            Type t when t == typeof(SaveEventData) => (
                ue => ue.SaveEvent != null,
                ue =>
                {
                    EnsureGroup(EventConsumerGroup.SaveEntity, offset.ConsumerGroup);
                    return ue.SaveEvent as T;
                }
            ),

            Type t when t == typeof(ViewEventData) => (
                (Expression<Func<UserEvent, bool>>)(ue => ue.ViewEvent != null),
                (Func<UserEvent, T?>)(ue =>
                {
                    EnsureGroup(EventConsumerGroup.ViewEntity, offset.ConsumerGroup);
                    return ue.ViewEvent as T;
                })
            ),

            _ => throw new NotSupportedException($"Unsupported event type: {typeof(T).Name}")
        };

        var materializedEvents = await query
            .Where(discriminator)
            .Where(ue => ue.Id > offset.LastProcessedEventId)
            .ToListAsync();

        return materializedEvents.CompactBy(selector);

        static void EnsureGroup(EventConsumerGroup expected, EventConsumerGroup actual)
        {
            if (expected != actual)
                throw new InvalidOperationException($"Expected {expected}, but got {actual}.");
        }
    }


    public static List<CompactionResult<T>> CompactBy<T>(
        this IEnumerable<UserEvent> events,
        Func<UserEvent, T?> selector)
        where T : class
        => events
            .Select(e => (Event: e, Data: selector(e)))
            .Where(x => x.Data is not null)
            .GroupBy(x => GetCompactionKey(x.Data!)) // <-- dynamic key
            .Select(g =>
            {
                var last = g
                    .OrderBy(x => x.Event.Id)
                    .Last();

                return new CompactionResult<T>(
                    EventId: last.Event.Id,
                    Data: last.Data!,
                    TotalCount: g.Count(),
                    Type: last.Event.ActionType
                );
            })
            .ToList();

    public static string GetCompactionKey(object data) => data switch
    {
        LikeEventData e => e.GetCompactionKey(),
        CommentEventData e => e.GetCompactionKey(),
        ViewEventData e => e.GetCompactionKey(),
        CommentLikeEventData e => e.GetCompactionKey(),
        SaveEventData e => e.GetCompactionKey(),
        FollowEventData e => e.GetCompactionKey(),
        _ => throw new InvalidOperationException("Unsupported event data type")
    };

}

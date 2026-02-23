using Clutch.Database;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.EntityFrameworkCore;

namespace Clutch.API.Features.Shared.Endpoints;

[Handler]
[MapGet("admin/clip-counters-audit")]
public static partial class RunCountersAudit
{
    public sealed record Query(bool OnlyMismatches = false);
    public sealed record Response(
        List<ClipStats> Clips);

    public sealed record ClipStats(
        int ClipId,
        AuditEntry ViewCount,
        AuditEntry CommentCount,
        AuditEntry LikeCount,
        AuditEntry SaveCount,
        AuditEntry ShareCount)
    {
        public bool HasMismatch =>
            !ViewCount.IsMatching ||
            !CommentCount.IsMatching ||
            !LikeCount.IsMatching ||
            !SaveCount.IsMatching ||
            !ShareCount.IsMatching;
    }

    public sealed record AuditEntry(int Actual, int Expected)
    {
        public bool IsMatching => Actual == Expected;
    }

    private static async ValueTask<Response> HandleAsync(
        Query query,
        ApplicationDbContext dbContext,
        CancellationToken token)
    {
        var clips = await dbContext.Clips
           .Include(c => c.Likes)
           .Include(c => c.Saves)
           .Include(c => c.Views)
           .Include(c => c.Shares)
           .Include(c => c.CommentThread)
               .ThenInclude(ct => ct.Comments)
           .ToListAsync(token);

        var results = clips.Select(clip => new ClipStats(
            ClipId: clip.Id,
            ViewCount: new AuditEntry(clip.ViewCount, clip.Views.Count),
            CommentCount: new AuditEntry(clip.CommentCount, clip.CommentThread.Comments.Count),
            LikeCount: new AuditEntry(
                clip.LikeCount,
                clip.Likes.GroupBy(l => new { l.AuthorId, l.ClipId }).Count()),
            SaveCount: new AuditEntry(
                clip.SaveCount,
                clip.Saves.GroupBy(s => new { s.AuthorId, s.ClipId }).Count()),
            ShareCount: new AuditEntry(clip.ShareCount, clip.Shares.Count)
        ));

        var filtered = query.OnlyMismatches
            ? results.Where(r => r.HasMismatch).ToList()
            : results.ToList();

        return new Response(filtered);
    }
}

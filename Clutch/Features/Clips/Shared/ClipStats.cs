namespace Clutch.Features.Clips.Shared;

public sealed record ClipStats(
    int ViewCount,
    int CommentCount,
    int ShareCount,
    int LikeCount,
    int SaveCount);
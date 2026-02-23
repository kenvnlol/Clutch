using System.ComponentModel.DataAnnotations.Schema;

namespace Clutch.Database.Entities.Clips;

public class ClipMedia
{
    public required BlobReference OriginalUpload { get; init; }
    public required BlobReference Avatar { get; init; }
    public BlobReference? Resolution144p { get; init; }
    public BlobReference? Resolution240p { get; init; }
    public BlobReference? Resolution360p { get; init; }
    public BlobReference? Resolution480p { get; init; }
    public BlobReference? Resolution720p { get; init; }
    public BlobReference? Resolution1080p { get; init; }

    [NotMapped]
    public Dictionary<VideoResolutionType, BlobReference> AvailableResolutions
        => new Dictionary<VideoResolutionType, BlobReference?>
            {
                { VideoResolutionType.P144, Resolution144p },
                { VideoResolutionType.P240, Resolution240p },
                { VideoResolutionType.P360, Resolution360p },
                { VideoResolutionType.P480, Resolution480p },
                { VideoResolutionType.P720, Resolution720p },
                { VideoResolutionType.P1080, Resolution1080p },
            }
            .Where(kv => kv.Value is not null)
            .ToDictionary(kv => kv.Key, kv => kv.Value!);
}
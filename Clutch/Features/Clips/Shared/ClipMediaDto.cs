using Clutch.Database.Entities.Clips;
using CommunityToolkit.Diagnostics;

namespace Clutch.Features.Clips.Shared;

public class ClipMediaDto
{
    public required string AvatarUri { get; init; }
    public required Dictionary<VideoResolutionType, string> Resolutions { get; init; }

    public static ClipMediaDto FromClipMedia(
        ClipMedia clipMedia,
        IConfiguration configuration)
    {
        var cdnUri = configuration["ClutchVideoStorage:CDNUri"];

        Guard.IsNotNull(cdnUri, nameof(cdnUri));

        return new ClipMediaDto
        {
            AvatarUri = MapUri(clipMedia.Avatar, cdnUri),
            Resolutions = clipMedia.AvailableResolutions.ToDictionary(
                kv => kv.Key,
                kv => MapUri(kv.Value, cdnUri)
            )
        };

        static string MapUri(BlobReference blob, string cdn) =>
            $"{cdn}{blob.ContainerName}/{blob.BlobName}";
    }
}

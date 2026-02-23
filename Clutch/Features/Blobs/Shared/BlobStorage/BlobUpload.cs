namespace Clutch.Features.Blobs.Shared.BlobStorage;

public record BlobUpload
{
    public required string BlobName { get; init; }

    public required string BlobUri { get; init; }

    public required IDictionary<string, string> Metadata { get; init; }
}

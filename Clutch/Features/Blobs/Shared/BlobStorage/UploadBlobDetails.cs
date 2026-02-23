namespace Clutch.Features.Blobs.Shared.BlobStorage;

public record UploadBlobDetails
{
    public BlobUpload? BlobUpload { get; init; }

    public bool IsTransientError { get; set; }

}

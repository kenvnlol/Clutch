using Clutch.Features.Blobs.Shared.BlobStorage;

namespace Clutch.Features.Blobs.Shared.FileUploadStrategy;

/// <summary>
/// This now has a dependency on blobstorage. So, will be difficult to replace unless made more modular. 
/// </summary>
public abstract class FileUploadStrategy
{
    internal readonly BlobStorageService _blobService;

    public FileUploadStrategy(BlobStorageService blobService)
    {
        _blobService = blobService;
    }

    public abstract Task<IReadOnlyList<UploadBlobDetails>> Upload(List<CustomFile> files);
    public async Task<UploadBlobDetails> Upload(CustomFile file)
        => (await Upload([file])).First();
    public abstract Task Delete(string containerName, string blobName);
}

using Clutch.Features.Blobs.Shared.BlobStorage;

namespace Clutch.Features.Blobs.Shared.FileUploadStrategy;

public class VideoUploadStrategy : FileUploadStrategy
{
    private readonly string _containerName = "videos";
    public VideoUploadStrategy(BlobStorageService blobStorageService) : base(blobStorageService)
    {
    }

    public override Task Delete(string containerName, string blobName)
    {
        throw new NotImplementedException();
    }

    public override async Task<IReadOnlyList<UploadBlobDetails>> Upload(List<CustomFile> files)
    {
        if (files.Count > 1)
        {
            throw new Exception("Can only upload one video at a time.");
        }

        List<UploadBlobDetails> videoDetails = [];

        var file = files.First();

        videoDetails.Add(await _blobService.UploadBlobAsync(_containerName, file.File.FileName, file.File.ContentType, file.File.OpenReadStream(), file.Metadata));

        return videoDetails;
    }
}

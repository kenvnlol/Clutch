using Clutch.Features.Blobs.Shared.BlobStorage;
using Clutch.Infrastructure.Services.BlobProcessors;

namespace Clutch.Features.Blobs.Shared.FileUploadStrategy;

public class ImageUploadStrategy : FileUploadStrategy
{
    private readonly ImageProcessorService _imageProcessorService;
    private readonly string _containerName = "images";
    public ImageUploadStrategy(BlobStorageService blobStorageService, ImageProcessorService imageProcessorService) : base(blobStorageService)
    {
        _imageProcessorService = imageProcessorService;
    }

    public override Task Delete(string containerName, string blobName)
    {
        throw new NotImplementedException();
    }

    public override async Task<IReadOnlyList<UploadBlobDetails>> Upload(List<CustomFile> files)
    {
        var processedImages = _imageProcessorService.MutateImages(files);

        List<UploadBlobDetails> imageDetails = [];

        foreach (var imageResult in processedImages)
        {
            using (imageResult.ImageStream)
            {
                imageDetails.Add(await _blobService.UploadBlobAsync(_containerName, imageResult.FileName, imageResult.ContentType, imageResult.ImageStream, imageResult.Metadata));
            }
        }
        return imageDetails;
    }
}

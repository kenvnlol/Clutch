using Clutch.Features.Blobs.Shared.FileUploadStrategy;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Clutch.Infrastructure.Services.BlobProcessors;

public class ImageProcessorService
{

    public IReadOnlyList<ProcessedImageResult> MutateImages(IEnumerable<CustomFile> imageFiles)
    {
        var processedImages = new List<ProcessedImageResult>();

        foreach (var fileData in imageFiles)
        {
            var processedImageStream = CompressImage(fileData.File.OpenReadStream());
            processedImages.Add(new ProcessedImageResult(processedImageStream, fileData.File.FileName, fileData.File.ContentType) { Metadata = fileData.Metadata });
        }

        return processedImages;
    }

    private MemoryStream CompressImage(Stream inputStream)
    {

        var resizedImageStream = new MemoryStream();

        using (var image = Image.Load(inputStream))
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(2048, 1366),
                Mode = ResizeMode.Max
            }));

            image.Save(resizedImageStream, new PngEncoder());

            resizedImageStream.Position = 0;
        }
        return resizedImageStream;
    }
}

public record ProcessedImageResult(MemoryStream ImageStream, string FileName, string ContentType)
{
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
};
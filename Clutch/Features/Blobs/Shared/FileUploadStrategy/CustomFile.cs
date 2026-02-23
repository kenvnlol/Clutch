namespace Clutch.Features.Blobs.Shared.FileUploadStrategy;

public record CustomFile(IFormFile File)
{
    public IDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
};


public class BlobReference
{

    public required string BlobName { get; init; }
    public required string ContainerName { get; init; }

    public Uri GetCDNUri(string baseUri)
        => new Uri($"{baseUri}/{ContainerName}/{BlobName}");
    public Uri GetBlobUri(string baseUri)
        => new Uri($"{baseUri}/{ContainerName}/{BlobName}");
}

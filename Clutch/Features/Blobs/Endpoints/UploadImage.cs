namespace Clutch.Features.Blobs.Endpoints;

//[Handler]
//[MapPost("/image")]
//public static partial class UploadImage
//{
//    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
//        => endpoint.DisableAntiforgery();

//    public record Command(IFormFile File);

//    public record BlobDto(int BlobId, string BlobUri);

//    private static async ValueTask<ActionResult<BlobDto>> HandleAsync([AsParameters] Command request, ApplicationDbContext dbContext, IClock clock, ImageUploadStrategy ImageUploadStrategy, CancellationToken token)
//    {
//        var uploadBlobDetails = await ImageUploadStrategy.Upload(new CustomFile(request.File) { Metadata = new Dictionary<string, string>() });

//        if (uploadBlobDetails.BlobUpload is null)
//        {
//            BlobUploadException.ThrowNotFoundException(uploadBlobDetails.IsTransientError ? BlobErrorCodes.TransientError : BlobErrorCodes.ServiceUnavailable);
//        }

//        var blob = new Blob
//        {
//            Name = uploadBlobDetails.BlobUpload.BlobName,
//            Uri = uploadBlobDetails.BlobUpload.BlobUri,
//            UploadedAt = clock.GetCurrentInstant(),
//            IsDefault = false
//        };

//        dbContext.Blobs.Add(blob);

//        await dbContext.SaveChangesAsync();

//        return new BlobDto(blob.Id, blob.Uri);
//    }
//}



using Clutch.Database;
using Clutch.Database.Entities.Blobs;
using Clutch.Features.Blobs;
using Clutch.Features.Blobs.Shared.FileUploadStrategy;
using Clutch.Infrastructure.Exceptions;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Clutch.API.Features.Blobs.Endpoints;

[Handler]
[MapPost("/video")]
public static partial class UploadVideo
{
    internal static void CustomizeEndpoint(IEndpointConventionBuilder endpoint)
        => endpoint.DisableAntiforgery();

    public record Command(
        IFormFile File);

    public record BlobDto(
        int BlobId,
        string BlobUri);

    private static async ValueTask<ActionResult<BlobDto>> HandleAsync(
        [AsParameters] Command command,
        ApplicationDbContext dbContext,
        VideoUploadStrategy videoUploadStrategy,
        CancellationToken token)
    {
        var uploadBlobDetails = await videoUploadStrategy.Upload(new CustomFile(command.File) { Metadata = new Dictionary<string, string>() });

        if (uploadBlobDetails.BlobUpload is null)
        {
            BlobUploadException.ThrowNotFoundException(uploadBlobDetails.IsTransientError ? BlobErrorCodes.TransientError : BlobErrorCodes.ServiceUnavailable);
        }

        var blob = new Blob
        {
            Name = uploadBlobDetails.BlobUpload.BlobName,
            Uri = uploadBlobDetails.BlobUpload.BlobUri,
            UploadedAt = DateTimeOffset.UtcNow,
            IsDefault = false
        };

        dbContext.Blobs.Add(blob);

        await dbContext.SaveChangesAsync();

        return new BlobDto(blob.Id, blob.Uri);
    }
}



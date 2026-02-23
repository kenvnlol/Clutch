using System.Diagnostics.CodeAnalysis;

namespace Clutch.Infrastructure.Exceptions;

public sealed class BlobUploadException(string blobErrorCode) : ExceptionBase("Blob storage was not available at this moment.", 400, blobErrorCode)
{
    [DoesNotReturn]
    public static void ThrowNotFoundException(string blobErrorCode) =>
        throw new BlobUploadException(blobErrorCode);
}


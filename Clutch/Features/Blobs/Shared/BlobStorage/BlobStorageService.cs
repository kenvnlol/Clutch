using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;

namespace Clutch.Features.Blobs.Shared.BlobStorage;

public class BlobStorageService
{

    private BlobServiceClient _blobServiceClient;
    private readonly ILogger<BlobStorageService> _logger;
    private static readonly ResiliencePipeline _pipeline = new ResiliencePipelineBuilder()
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = static args =>
            {
                if (args.Outcome.Result is Response<BlobContentInfo> response)
                {
                    return ValueTask.FromResult(response.GetRawResponse().Status != 201);
                }

                return ValueTask.FromResult(false);
            },
            BackoffType = DelayBackoffType.Exponential,
            Delay = TimeSpan.FromSeconds(1),
            MaxRetryAttempts = 4,
            UseJitter = true
        })
        .AddRetry(new RetryStrategyOptions
        {
            ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
            BackoffType = DelayBackoffType.Exponential,
            MaxDelay = TimeSpan.FromMinutes(1),
            Delay = TimeSpan.FromSeconds(2),
            MaxRetryAttempts = 5,
            UseJitter = true
        })
        .AddCircuitBreaker(new CircuitBreakerStrategyOptions
        {
            FailureRatio = 0.5,
            SamplingDuration = TimeSpan.FromSeconds(30),
            MinimumThroughput = 10,
            BreakDuration = TimeSpan.FromSeconds(60)
        })
        .Build();
    public BlobStorageService(BlobServiceClient blobServiceClient, ILogger<BlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient ?? throw new Exception($"Can't find {nameof(blobServiceClient)}");
        _logger = logger;
    }

    // Check if blob exists for deletion

    public async Task<IReadOnlyList<string>> ListBlobsInContainer(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        var pageableBlobs = containerClient.GetBlobsAsync();
        var blobUris = new List<string>();

        var enumerator = pageableBlobs.GetAsyncEnumerator();

        try
        {
            while (await enumerator.MoveNextAsync())
            {
                var blobName = enumerator.Current.Name;
                var blobUri = containerClient.GetBlobClient(blobName).Uri.ToString();
                blobUris.Add(blobUri);
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
        }

        return blobUris;
    }

    public string GetStorageAccountName()
        => _blobServiceClient.AccountName;


    public async Task CreateContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        if (await containerClient.ExistsAsync())
            throw new ApplicationException($"Unable to create container '{containerName}' as it already exists");

        await containerClient.CreateAsync();
    }

    public async Task DeleteContainerAsync(string containerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        if (!await containerClient.ExistsAsync())
            throw new ApplicationException($"Unable to delete container '{containerName}' as it does not exists");

        await containerClient.DeleteAsync();
    }

    // Maybe rename since it also returns the URI
    public async Task<UploadBlobDetails> UploadBlobAsync(string containerName, string fileName, string contentType, Stream content, IDictionary<string, string> metadata)
    {
        var blobName = $"{Guid.NewGuid()}{fileName}";
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

        if (!await containerClient.ExistsAsync())
        {
            throw new ApplicationException($"Unable to upload blobs to container '{containerName}' as the container does not exists");
        }

        var blobClient = containerClient.GetBlobClient(blobName);
        var options = new BlobUploadOptions() { HttpHeaders = new BlobHttpHeaders() { ContentType = contentType }, Metadata = metadata };
        try
        {
            var blobContentInfo = await _pipeline.ExecuteAsync(async x => await blobClient.UploadAsync(content, options));

            var statusCode = blobContentInfo.GetRawResponse().Status;
            var reasonPhrase = blobContentInfo.GetRawResponse().ReasonPhrase;

            if (statusCode != 201)
            {
                _logger.LogError(
                    "Failed to upload blob {BlobName}. Status code: {StatusCode}, Reason: {ReasonPhrase}",
                    blobName,
                    statusCode,
                    reasonPhrase);

                return new UploadBlobDetails
                {
                    BlobUpload = null,
                    IsTransientError = false
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(
                ex,
                "Request failed while uploading blob {BlobName}. Message: {Message}",
                blobName,
                ex.Message);


            return new UploadBlobDetails
            {
                BlobUpload = null,
                IsTransientError = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(
            ex,
            "Request failed while uploading blob {BlobName}. Message: {Message}",
            blobName,
            ex.Message);


            return new UploadBlobDetails
            {
                BlobUpload = null,
                IsTransientError = false
            };
        }

        return new UploadBlobDetails
        {
            BlobUpload = new BlobUpload
            {
                BlobName = blobName,
                BlobUri = blobClient.Uri.ToString(),
                Metadata = metadata
            },
            IsTransientError = false
        };
    }


    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        if (!await containerClient.ExistsAsync())
            throw new ApplicationException($"Unable to delete blob {blobName} in container '{containerName}' as the container does not exists");

        var blobClient = containerClient.GetBlobClient(blobName);
        if (!await blobClient.ExistsAsync())
            throw new ApplicationException($"Unable to delete blob {blobName} in container '{containerName}' as no blob with this name exists in this container");

        await blobClient.DeleteAsync();
    }

    public async Task DeleteBlobByUri(string blobUrl)
    {
        var blobClient = new BlobClient(new Uri(blobUrl));
        bool blobExists;

        try
        {
            blobExists = await blobClient.ExistsAsync();
        }
        catch (RequestFailedException ex)
        {
            _logger.LogWarning("Failed to check if blob exists. Uri: {BlobUri}, Error: {ErrorMessage}", blobUrl, ex.Message);
            throw;
        }
        ;

        if (!blobExists)
        {
            _logger.LogWarning("Blob did not exist. Uri: {BlobUri}", blobUrl);
            throw new ApplicationException("Blob did not exist.");
        }

        await blobClient.DeleteAsync();
    }
}

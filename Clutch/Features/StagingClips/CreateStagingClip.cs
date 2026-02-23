using Clutch.Database;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.StagingClips;
using Clutch.Features.Users.Services;
using CommunityToolkit.Diagnostics;
using IdGen;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;

namespace Clutch.Features.Features.StagingClips;

// TODO: don't hardcode raw-videos
[Handler]
[MapPost("/staging-clip")]
public static partial class CreateStagingClip
{
    public sealed record Command(
        int GameId,
        string Description);
    public sealed record BlobDto(
        string Uri,
        string Id);
    private static ValueTask HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        UserService userService,
        IdGenerator idGen,
        IConfiguration configuration,
        CancellationToken token)
    {
        var currentUserId = userService.GetCurrentUserId();
        var blobName = $"{currentUserId}:{idGen.CreateId()}";
        var containerName = configuration["ClutchVideoStorage:Containers:RawVideos"];

        Guard.IsNotNull(containerName, nameof(containerName));

        var stagingClip = new StagingClip
        {
            BlobReference = new BlobReference
            {
                BlobName = blobName,
                ContainerName = containerName
            },
            AuthorId = currentUserId,
            Description = command.Description,
            Timestamp = DateTimeOffset.UtcNow,
            GameId = command.GameId
        };

        dbContext.StagingClips.Add(stagingClip);
        dbContext.SaveChangesAsync();

        return ValueTask.CompletedTask;
    }
}


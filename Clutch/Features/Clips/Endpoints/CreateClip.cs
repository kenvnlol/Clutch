using Clutch.Database;
using Clutch.Database.Entities.Clips;
using Clutch.Database.Entities.CommentThreads;
using Immediate.Apis.Shared;
using Immediate.Handlers.Shared;
using LinqToDB;
using Microsoft.AspNetCore.Mvc;

namespace Clutch.API.Features.Clips;


[Handler]
[MapPost("/transcoding-complete")]
//[Authorize(Policy = "TranscodingCompletePolicy")]
public static partial class CreateClip
{
    public sealed record Command(
        ClipMedia ClipMedia);

    public sealed record ClipResolutionDetails(VideoResolutionType ResolutionType,
                                               string ContainerName,
                                               string BlobName);

    private static async ValueTask<ActionResult> HandleAsync(
        Command command,
        ApplicationDbContext dbContext,
        CancellationToken token)
    {
        if (await dbContext.Clips.AnyAsync(clip =>
            clip.Media.OriginalUpload.BlobName == command.ClipMedia.OriginalUpload.BlobName))
        {
            return new OkObjectResult("A clip with the same upload already exists. Duplicate uploads are not allowed.");
        }

        var stagingClip = await dbContext.StagingClips
             .FirstOrDefaultAsync(clip =>
                 clip.BlobReference.BlobName == command.ClipMedia.OriginalUpload.BlobName &&
                 clip.BlobReference.ContainerName == command.ClipMedia.OriginalUpload.ContainerName);

        if (stagingClip is null)
        {
            return new NotFoundObjectResult($"No staging clip found for blob '{command.ClipMedia.OriginalUpload.BlobName}'.");
        }

        var clip = new Clip
        {
            Description = stagingClip.Description,
            CommentThread = new CommentThread
            {
                Comments = []
            },
            AuthorId = stagingClip.AuthorId,
            GameId = stagingClip.GameId,
            Likes = [],
            Views = [],
            Shares = [],
            Saves = [],
            ContestWins = [],
            Timestamp = DateTimeOffset.UtcNow,
            Media = command.ClipMedia
        };

        dbContext.StagingClips.Remove(stagingClip);
        dbContext.Clips.Add(clip);
        await dbContext.SaveChangesAsync();

        return new OkResult();
    }
}


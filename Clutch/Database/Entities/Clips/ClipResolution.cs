using Clutch.Database.Entities.Clips;

public class ClipResolution : BlobReference
{
    public required VideoResolutionType Type { get; init; }
}

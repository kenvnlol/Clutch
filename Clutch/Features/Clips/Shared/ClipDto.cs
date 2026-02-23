namespace Clutch.Features.Clips.Shared;

public sealed record ClipDto(
     int Id,
     bool Liked,
     bool Saved,
     string Description,
     string UploadDate,
     ClipMediaDto ClipMedia,
     ClipAuthorDto Author,
     ClipStats Stats,
     string? externalVideoUri);



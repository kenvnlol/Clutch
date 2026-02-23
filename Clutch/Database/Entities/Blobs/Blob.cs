namespace Clutch.Database.Entities.Blobs;

/// <summary>
/// Represents a system-owned blob stored in the system's storage. 
/// 
/// - Blobs are not directly owned by any user but are managed by the system.
/// - Only administrators have full CRUD (Create, Read, Update, Delete) permissions for blobs.
/// - Regular users can upload blobs but cannot modify or delete them directly.
/// - Once uploaded, blobs are referenced in entity resources (e.g., restaurant images, menu items).
/// - The system is responsible for cleaning up unreferenced blobs.
/// </summary>
public class Blob
{
    public int Id { get; init; }
    public required string Name { get; set; }
    public required string Uri { get; set; }
    public required DateTimeOffset UploadedAt { get; set; }
    public required bool IsDefault { get; init; }
}

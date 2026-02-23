using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.StagingClips;

public class StagingClipConfiguration : IEntityTypeConfiguration<StagingClip>
{
    public void Configure(EntityTypeBuilder<StagingClip> builder)
    {
        builder.OwnsOne(c => c.BlobReference);
    }
}

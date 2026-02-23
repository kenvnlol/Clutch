using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.Clips;

public class ClipConfiguration : IEntityTypeConfiguration<Clip>
{
    public void Configure(EntityTypeBuilder<Clip> builder)
    {
        builder.OwnsOne(c => c.Media, media =>
        {
            media.OwnsOne(m => m.OriginalUpload);
            media.OwnsOne(m => m.Avatar);

            media.OwnsOne(m => m.Resolution144p);
            media.OwnsOne(m => m.Resolution240p);
            media.OwnsOne(m => m.Resolution360p);
            media.OwnsOne(m => m.Resolution480p);
            media.OwnsOne(m => m.Resolution720p);
            media.OwnsOne(m => m.Resolution1080p);
        });
    }
}

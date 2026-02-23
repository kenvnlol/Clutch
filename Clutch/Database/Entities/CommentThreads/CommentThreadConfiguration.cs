using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.CommentThreads;

public class CommentThreadConfiguration : IEntityTypeConfiguration<CommentThread>
{
    public void Configure(EntityTypeBuilder<CommentThread> builder)
    {
        builder.HasKey(builder => builder.ClipId);

        builder.HasOne(thread => thread.Clip)
               .WithOne(clip => clip.CommentThread)
               .HasForeignKey<CommentThread>(thread => thread.ClipId);
    }
}

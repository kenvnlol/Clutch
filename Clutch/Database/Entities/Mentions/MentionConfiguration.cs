using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.Mentions;

public class MentionConfiguration : IEntityTypeConfiguration<Mention>
{
    public void Configure(EntityTypeBuilder<Mention> builder)
    {
        builder.HasOne(f => f.InitiatorUser)
            .WithMany(u => u.MentionsMade)
            .HasForeignKey(f => f.InitiatorUserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(f => f.TargetUser)
            .WithMany(u => u.MentionsReceived)
            .HasForeignKey(f => f.TargetUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}




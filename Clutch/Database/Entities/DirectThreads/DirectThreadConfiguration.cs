using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.DirectThreads;

public class DirectThreadConfiguration : IEntityTypeConfiguration<DirectThread>
{
    public void Configure(EntityTypeBuilder<DirectThread> builder)
    {
        builder
            .HasIndex(c => new { c.ParticipantAId, c.ParticipantBId })
            .IsUnique();

        builder
            .HasOne(dt => dt.ParticipantA)
            .WithMany()
            .HasForeignKey(dt => dt.ParticipantAId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(dt => dt.ParticipantB)
            .WithMany()
            .HasForeignKey(dt => dt.ParticipantBId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

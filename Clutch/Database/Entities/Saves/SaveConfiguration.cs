using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.Saves;

public class SaveConfiguration : IEntityTypeConfiguration<Save>
{
    public void Configure(EntityTypeBuilder<Save> builder)
    {
        builder.HasIndex(c => new { c.AuthorId, c.ClipId }).IsUnique();
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.InboxActivities;
public class InboxActivityConfiguration : IEntityTypeConfiguration<InboxActivity>
{
    public void Configure(EntityTypeBuilder<InboxActivity> builder)
    {
        builder.HasQueryFilter(comment => !comment.IsDeleted);
    }
}

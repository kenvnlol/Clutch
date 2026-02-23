using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.UserInboxes;

public class UserInboxConfiguration : IEntityTypeConfiguration<UserInbox>
{
    public void Configure(EntityTypeBuilder<UserInbox> builder)
    {
        builder
            .HasKey(builder => builder.UserId);

        builder
            .HasOne(inbox => inbox.User)
            .WithOne(user => user.UserInbox)
            .HasForeignKey<UserInbox>(inbox => inbox.UserId);
    }
}

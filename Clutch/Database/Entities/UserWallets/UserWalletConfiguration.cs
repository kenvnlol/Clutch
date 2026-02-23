using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clutch.Database.Entities.UserWallets;

public class UserWalletConfiguration : IEntityTypeConfiguration<UserWallet>
{
    public void Configure(EntityTypeBuilder<UserWallet> builder)
    {
        builder
        .HasKey(builder => builder.UserId);

        builder
            .HasOne(wallet => wallet.User)
            .WithOne(user => user.Wallet)
            .HasForeignKey<UserWallet>(wallet => wallet.UserId);
    }
}

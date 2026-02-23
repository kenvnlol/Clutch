using Clutch.Database.Entities.Users;

namespace Clutch.Database.Entities.UserWallets;

public sealed class UserWallet
{
    public string UserId { get; init; } = null!;
    public User User { get; init; } = null!;
    public decimal Balance { get; set; } = 0m;
    public decimal TotalEarned { get; set; } = 0m;
    public required List<WithdrawalRequest> WithdrawalRequests { get; init; }
}

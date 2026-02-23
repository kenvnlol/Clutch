namespace Clutch.Database.Entities.UserWallets;

public sealed class WithdrawalRequest
{
    public int Id { get; set; }
    public required string UserWalletId { get; init; }
    public required UserWallet UserWallet { get; init; } = null!;
    public required decimal Amount { get; set; }
    public required DateTimeOffset RequestedAt { get; init; }
    public required WithdrawalStatus Status { get; set; }
    public required WithdrawalDestination Destination { get; init; }
}


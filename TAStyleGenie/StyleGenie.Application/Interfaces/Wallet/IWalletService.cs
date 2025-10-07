namespace StyleGenie.Application.Wallet;

public interface IWalletService
{
    Task<bool> TryDebitAsync(long tenantId, int credits, string reason, long? jobId, string? note);
    Task CreditAsync(long tenantId, int credits, string reason, string? note);
}

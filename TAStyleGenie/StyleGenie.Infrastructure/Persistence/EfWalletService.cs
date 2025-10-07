using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Wallet;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Infrastructure.Persistence;

public sealed class EfWalletService : IWalletService
{
    private readonly TryOnDbContext _db;
    public EfWalletService(TryOnDbContext db) => _db = db;

    public async Task<bool> TryDebitAsync(long tenantId, int credits, string reason, long? jobId, string? note)
    {
        if (credits <= 0) throw new ArgumentOutOfRangeException(nameof(credits));
        using var tx = await _db.Database.BeginTransactionAsync();

        var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.TenantId == tenantId);
        if (wallet == null)
        {
            wallet = new CreditWallet { TenantId = tenantId, Balance = 0, UpdatedAt = DateTime.UtcNow };
            _db.CreditWallets.Add(wallet);
            await _db.SaveChangesAsync();
        }

        if (wallet.Balance < credits) return false;

        wallet.Balance -= credits; wallet.UpdatedAt = DateTime.UtcNow;
        _db.UsageLedgers.Add(new UsageLedger
        {
            TenantId = tenantId,
            JobId = jobId,
            Credits = -credits,
            Reason = reason,
            Note = note,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
        return true;
    }

    public async Task CreditAsync(long tenantId, int credits, string reason, string? note)
    {
        if (credits <= 0) throw new ArgumentOutOfRangeException(nameof(credits));
        using var tx = await _db.Database.BeginTransactionAsync();

        var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.TenantId == tenantId);
        if (wallet == null)
        {
            wallet = new CreditWallet { TenantId = tenantId, Balance = 0, UpdatedAt = DateTime.UtcNow };
            _db.CreditWallets.Add(wallet);
        }

        wallet.Balance += credits; wallet.UpdatedAt = DateTime.UtcNow;
        _db.UsageLedgers.Add(new UsageLedger
        {
            TenantId = tenantId,
            JobId = null,
            Credits = credits,
            Reason = reason,
            Note = note,
            CreatedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        await tx.CommitAsync();
    }
}

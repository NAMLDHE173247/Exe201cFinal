using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StyleGenie.Infrastructure.Data.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace StyleGenie.Infrastructure.Services
{
    public class CreditResetService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<CreditResetService> _logger;

        public CreditResetService(IServiceProvider serviceProvider, ILogger<CreditResetService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[CreditReset] Service started at: {time}", DateTimeOffset.Now);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var now = DateTime.Now;
                    var nextRun = DateTime.Today.AddDays(1); // chạy mỗi 00:00

                    await ResetCredits(stoppingToken);

                    var delay = nextRun - now;
                    _logger.LogInformation("[CreditReset] Next reset scheduled at {nextRun}", nextRun);

                    await Task.Delay(delay, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[CreditReset] ❌ Error in service loop");
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken); // chờ 1h rồi thử lại
                }
            }
        }

        private async Task ResetCredits(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TryOnDbContext>();

            var users = await db.Users
                .Where(u => u.TenantId != null)
                .ToListAsync(ct);

            _logger.LogInformation("[CreditReset] Found {Count} users to process", users.Count);

            foreach (var user in users)
            {
                try
                {
                    int planId = (int)(user.SubscriptionPlanId ?? 0);

                    int resetBalance = planId switch
                    {
                        1 => 3,   // Basic
                        2 => 15,  // Premium
                        3 => 50,  // VIP
                        _ => 3    // Mặc định
                    };

                    var wallet = await db.CreditWallets.FirstOrDefaultAsync(w => w.TenantId == user.TenantId, ct);

                    if (wallet != null)
                    {
                        wallet.Balance = resetBalance;
                        wallet.UpdatedAt = DateTime.UtcNow;
                        db.CreditWallets.Update(wallet);
                        _logger.LogInformation("[CreditReset] Updated Tenant {TenantId} → {Balance} credits", user.TenantId, resetBalance);
                    }
                    else
                    {
                        var newWallet = new CreditWallet
                        {
                            TenantId = user.TenantId,
                            Balance = resetBalance,
                      //      CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        db.CreditWallets.Add(newWallet);
                        _logger.LogInformation("[CreditReset] Created new wallet for Tenant {TenantId} with {Balance} credits", user.TenantId, resetBalance);
                    }

                    await db.SaveChangesAsync(ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[CreditReset] Error updating user {UserId}", user.Id);
                }
            }

            _logger.LogInformation("[CreditReset] ✅ Finished reset at {Time}", DateTime.UtcNow);
        }
    }
}

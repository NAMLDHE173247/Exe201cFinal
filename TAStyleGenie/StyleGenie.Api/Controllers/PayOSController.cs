using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using StyleGenie.Infrastructure.Data.Models;
using System.ComponentModel.DataAnnotations;
using TryOn.Api.Services;

namespace TryOn.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayOSController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        private readonly PayOSService _payos;
        private readonly ILogger<PayOSController> _logger;

        public PayOSController(TryOnDbContext db, PayOSService payos, ILogger<PayOSController> logger)
        {
            _db = db;
            _payos = payos;
            _logger = logger;
        }

        // ✅ DTO khi frontend tạo thanh toán
        public class CreatePaymentRequest
        {
            [Required] public long UserId { get; set; }
            [Required] public long PlanId { get; set; }
            [Required] public string CancelUrl { get; set; } = string.Empty;
            [Required] public string ReturnUrl { get; set; } = string.Empty;
        }

        // ================================
        // 1️⃣ TẠO LINK THANH TOÁN
        // ================================
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentRequest req)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _db.Users.FindAsync(req.UserId);
            var plan = await _db.SubscriptionPlans.FindAsync(req.PlanId);

            if (user == null || plan == null)
                return BadRequest("User hoặc gói không tồn tại.");

            // Lưu giao dịch
            var txn = new Transaction
            {
                UserId = user.Id,
                PlanId = plan.Id,
                Type = "SUBSCRIBE",
                Amount = plan.Price ?? 0,
                Status = "PENDING",
                Description = $"{plan.Name} cho {user.Id}",
                CreatedAt = DateTime.UtcNow
            };
            _db.Transactions.Add(txn);
            await _db.SaveChangesAsync();

            var orderCode = txn.Id;
            _logger.LogInformation("[PayOS] Creating payment link for txn {TxnId} (User {UserId}, Plan {PlanId})", txn.Id, user.Id, plan.Id);

            var result = await _payos.CreatePaymentLinkAsync(
                orderCode,
                plan.Price ?? 0,
                $"Gói {plan.Name} - {user.Id}",
                req.CancelUrl,
                req.ReturnUrl
            );

            var data = result["data"] as JObject;
            if (data == null)
            {
                _logger.LogWarning("[PayOS] Invalid response: {Raw}", result.ToString());
                return BadRequest(new { message = "Invalid response from PayOS", raw = result });
            }

            txn.PayOspaymentLinkId = data["paymentLinkId"]?.ToString();
            txn.PayOscheckoutUrl = data["checkoutUrl"]?.ToString();
            txn.PayOsresponseJson = result.ToString();
            await _db.SaveChangesAsync();

            _logger.LogInformation("[PayOS] ✅ Payment link created: {Url}", txn.PayOscheckoutUrl);

            return Ok(new
            {
                code = "00",
                message = "Success",
                checkoutUrl = txn.PayOscheckoutUrl,
                transactionId = txn.Id,
                planName = plan.Name,
                price = plan.Price
            });
        }

        // ================================
        // 2️⃣ PAYOS WEBHOOK CALLBACK
        // ================================
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            using var reader = new StreamReader(Request.Body);
            var rawBody = await reader.ReadToEndAsync();

            _logger.LogInformation("[PayOS] Webhook raw: {Raw}", rawBody);

            var payload = JObject.Parse(rawBody);
            var signature = payload["signature"]?.ToString() ?? "";
            var dataJson = payload["data"]?.ToString(Newtonsoft.Json.Formatting.None) ?? "{}";
            var expected = _payos.ComputeSignature(dataJson);

            if (!string.Equals(signature, expected, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("[PayOS] ❌ Invalid signature. Expected: {Expected}, Got: {Actual}", expected, signature);
                return BadRequest("Invalid signature");
            }

            var data = JObject.Parse(dataJson);
            var paymentLinkId = data["paymentLinkId"]?.ToString() ?? "";
            var code = data["code"]?.ToString() ?? "";
            var status = code == "00" ? "PAID" : "FAILED";

            var txn = await _db.Transactions.FirstOrDefaultAsync(t => t.PayOspaymentLinkId == paymentLinkId);
            if (txn == null)
            {
                _logger.LogWarning("[PayOS] No transaction found for linkId {PaymentLinkId}", paymentLinkId);
                return Ok(new { message = "No transaction found" });
            }

            if (txn.Status == "PAID")
                return Ok(new { message = "Already processed" });

            txn.Status = status;
            txn.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();

            if (status == "PAID")
            {
                var user = await _db.Users.FindAsync(txn.UserId);
                var plan = await _db.SubscriptionPlans.FindAsync(txn.PlanId);

                if (user != null && plan != null)
                {
                    user.SubscriptionPlanId = plan.Id;

                    var vip = new VipPayment
                    {
                        UserId = user.Id,
                        PlanId = plan.Id,
                        TransactionId = txn.Id,
                        StartDate = DateOnly.FromDateTime(DateTime.UtcNow),
                        EndDate = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(1))
                    };

                    _db.VipPayments.Add(vip);
                    await _db.SaveChangesAsync();

                    _logger.LogInformation("[PayOS] 🎉 User {UserId} activated plan {PlanId}", user.Id, plan.Id);

                    // 💰 Cập nhật CreditWallet
                    var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.TenantId == user.TenantId);

                    int newBalance = plan.Id switch
                    {
                        1 => 3,   // Basic
                        2 => 15,  // Premium
                        3 => 50,  // VIP
                        _ => 3
                    };

                    if (wallet != null)
                    {
                        wallet.Balance = newBalance;
                        wallet.UpdatedAt = DateTime.UtcNow;
                        _db.CreditWallets.Update(wallet);
                        await _db.SaveChangesAsync();

                        _logger.LogInformation("[Wallet] 💰 Updated existing wallet for Tenant {TenantId} to {Balance}", user.TenantId, newBalance);
                    }
                    else
                    {
                        var newWallet = new CreditWallet
                        {
                            TenantId = user.TenantId,
                            Balance = newBalance,
                            //       CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow
                        };
                        _db.CreditWallets.Add(newWallet);
                        await _db.SaveChangesAsync();

                        _logger.LogInformation("[Wallet] 🆕 Created new wallet for Tenant {TenantId} with {Balance}", user.TenantId, newBalance);
                    }
                }
            }

            return Ok(new { message = "Webhook processed", status });
        }

        // ================================
        // 3️⃣ RETURN CALLBACK CHO CLIENT
        // ================================
        [HttpGet("return")]
        public async Task<IActionResult> Return([FromQuery] long orderCode, [FromQuery] string status)
        {
            var txn = await _db.Transactions.FirstOrDefaultAsync(t => t.Id == orderCode);
            if (txn == null)
                return NotFound(new { message = "Transaction not found", status = "ERROR" });

            if (txn.Status == "PENDING")
            {
                txn.Status = status?.ToUpper() == "PAID" ? "PAID" : "FAILED";
                txn.UpdatedAt = DateTime.UtcNow;
                await _db.SaveChangesAsync();
            }

            return Ok(new
            {
                message = txn.Status == "PAID" ? "Payment success" : "Payment failed",
                status = txn.Status
            });
        }

        // ================================
        // 4️⃣ DANH SÁCH GIAO DỊCH
        // ================================
        [HttpGet("list")]
        public IActionResult List()
        {
            var list = _db.Transactions
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new
                {
                    t.Id,
                    t.UserId,
                    t.PlanId,
                    t.Amount,
                    t.Status,
                    t.PayOspaymentLinkId,
                    t.PayOscheckoutUrl,
                    t.CreatedAt
                })
                .ToList();

            return Ok(list);
        }
    }
}

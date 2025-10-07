using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Dto.TryOn;
using StyleGenie.Application.TryOn;
using StyleGenie.Infrastructure.Data.Models;
using StyleGenie.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers;

[ApiController]
[Route("api/vto")]
[AllowAnonymous] // ✅ Bỏ auth, gọi thoải mái
public class TryOnController : ControllerBase
{
    private readonly ITryOnService _svc;
    private readonly TryOnDbContext _db;

    public TryOnController(ITryOnService svc, TryOnDbContext db)
    {
        _svc = svc;
        _db = db;
    }

    // ✅ Thử đồ
    [HttpPost("tryon")]
    [AllowAnonymous]
    [SwaggerOperation(
        Summary = "Thử đồ (FitRoom)",
        Description = "Gửi model/cloth (base64 → byte[]) và nhận ảnh kết quả base64.")]
    public async Task<ActionResult<object>> TryOn([FromBody] TryOnRequestDto dto, CancellationToken ct)
    {
        // Kiểm tra tenantId hợp lệ
        if (dto.TenantId <= 0)
            return BadRequest(new { error = "tenantId is required" });

        // Kiểm tra số dư credit của tenant
        var wallet = await _db.CreditWallets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == dto.TenantId, ct);

        if (wallet == null || wallet.Balance <= 0)
            return BadRequest(new { error = "Insufficient credits" });

        // Gọi service TryOnAsync để thực hiện thử đồ
        var res = await _svc.TryOnAsync(dto, ct);

        // Trả về kết quả thử đồ dưới dạng base64
        return Ok(new
        {
            res.JobId,
            ImageBase64 = Convert.ToBase64String(res.ResultJpeg)
        });
    }

    // ✅ Xem số dư
    [HttpGet("wallet/balance")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Xem số dư credit của tenant")]
    public async Task<IActionResult> GetBalance([FromQuery] long tenantId, CancellationToken ct)
    {
        // Kiểm tra tenantId hợp lệ
        if (tenantId <= 0)
            return BadRequest(new { error = "tenantId is required" });

        // Truy vấn số dư balance của tenant
        var wallet = await _db.CreditWallets.AsNoTracking()
            .FirstOrDefaultAsync(x => x.TenantId == tenantId, ct);

        return Ok(new
        {
            tenantId,
            balance = wallet?.Balance ?? 0
        });
    }
}

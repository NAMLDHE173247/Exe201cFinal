using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;

[ApiController]
[Route("api/test-ef")]
public class TestEfController : ControllerBase
{
    private readonly TryOnDbContext _db;
    public TestEfController(TryOnDbContext db) => _db = db;

    [HttpGet("tenants/count")]
    public async Task<IActionResult> CountTenants()
    {
        var count = await _db.Tenants.CountAsync();
        return Ok(new { tenants = count });
    }
}

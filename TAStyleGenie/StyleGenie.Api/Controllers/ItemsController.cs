using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly TryOnDbContext _context;

        public ItemController(TryOnDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // GET: api/item
        // ==========================================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.Items
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return Ok(items.Select(i => new
            {
                i.Id,
                i.Name,
                i.Price,
                i.Material,
                i.Category,
                i.AffiliateUrl,
                i.IsVipOnly,
                i.IsActive,
                i.CreatedAt,
                ImageBase64 = i.ImageBase64 != null ? $"data:image/jpeg;base64,{i.ImageBase64}" : null
            }));
        }

        // ==========================================================
        // GET: api/item/{id}
        // ==========================================================
        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetById(long id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();

            return Ok(new
            {
                item.Id,
                item.Name,
                item.Price,
                item.Material,
                item.Category,
                item.AffiliateUrl,
                item.IsVipOnly,
                item.IsActive,
                item.CreatedAt,
                ImageBase64 = item.ImageBase64 != null ? $"data:image/jpeg;base64,{item.ImageBase64}" : null
            });
        }

        // ==========================================================
        // POST: api/item
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ItemCreateDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                string? base64Image = null;
                if (dto.ImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await dto.ImageFile.CopyToAsync(ms);
                    base64Image = Convert.ToBase64String(ms.ToArray());
                }

                var item = new Item
                {
                    Name = dto.Name ?? "(No Name)",
                    Price = dto.Price,
                    Material = dto.Material,
                    Category = dto.Category,
                    AffiliateUrl = dto.AffiliateUrl,
                    ImageBase64 = base64Image,
                    IsVipOnly = dto.IsVipOnly,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Items.Add(item);
                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, new { ok = true, id = item.Id });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Create item failed", error = ex.Message });
            }
        }

        // ==========================================================
        // PUT: api/item/{id}
        // ==========================================================
        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, [FromForm] ItemUpdateDto dto)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
                if (item == null) return NotFound();

                item.Name = dto.Name ?? item.Name;
                item.Price = dto.Price ?? item.Price;
                item.Material = dto.Material ?? item.Material;
                item.Category = dto.Category ?? item.Category;
                item.AffiliateUrl = dto.AffiliateUrl ?? item.AffiliateUrl;
                item.IsVipOnly = dto.IsVipOnly ?? item.IsVipOnly;
                item.IsActive = dto.IsActive ?? item.IsActive;

                if (dto.ImageFile != null)
                {
                    using var ms = new MemoryStream();
                    await dto.ImageFile.CopyToAsync(ms);
                    item.ImageBase64 = Convert.ToBase64String(ms.ToArray());
                }

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new { updated = true });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Update item failed", error = ex.Message });
            }
        }

        // ==========================================================
        // DELETE: api/item/{id}
        // ==========================================================
        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            var item = await _context.Items.FirstOrDefaultAsync(i => i.Id == id);
            if (item == null) return NotFound();

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ==========================================================
        // DTOs
        // ==========================================================
        public class ItemCreateDto
        {
            public string? Name { get; set; }
            public decimal? Price { get; set; }
            public string? Material { get; set; }
            public string? Category { get; set; }
            public string? AffiliateUrl { get; set; }
            public bool IsVipOnly { get; set; }
            public IFormFile? ImageFile { get; set; }
        }

        public class ItemUpdateDto
        {
            public string? Name { get; set; }
            public decimal? Price { get; set; }
            public string? Material { get; set; }
            public string? Category { get; set; }
            public string? AffiliateUrl { get; set; }
            public bool? IsVipOnly { get; set; }
            public bool? IsActive { get; set; }
            public IFormFile? ImageFile { get; set; }
        }
    }
}

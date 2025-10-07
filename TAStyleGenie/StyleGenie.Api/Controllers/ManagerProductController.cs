using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StyleGenie.Infrastructure.Data.Models;
using Swashbuckle.AspNetCore.Annotations;

namespace StyleGenie.Api.Controllers
{
    // ✅ DTO cho danh sách
    public record ProductListItemDto(
        long Id,
        string Name,
        decimal? Price,
        string? Material,        // Phong cách
        string? Category,        // Loại sản phẩm
        string? AffiliateUrl,
        bool IsVipOnly,
        bool IsActive,
        DateTime CreatedAt,
        string? ImageBase64      // prefix data:image/...
    );

    // ✅ DTO tạo mới (multipart/form-data)
    public class ItemCreateDto
    {
        public string? Name { get; set; }
        public decimal? Price { get; set; }

        // Cho phép nhập mới hoặc chọn từ dropdown
        public string? Material { get; set; }
        public string? Category { get; set; }

        public string? AffiliateUrl { get; set; }
        public bool IsVipOnly { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    [Authorize(Policy = "StaffOrAdmin")]
    [ApiController]
    [Route("api/[controller]")]
    public class ManagerProductController : ControllerBase
    {
        private readonly TryOnDbContext _db;
        public ManagerProductController(TryOnDbContext db) => _db = db;

        // =====================================================
        // GET /api/ManagerProduct/{id}
        // =====================================================
        [HttpGet("{id:long}")]
        [SwaggerOperation(Summary = "Lấy chi tiết 1 sản phẩm theo Id")]
        [ProducesResponseType(typeof(ProductListItemDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(long id, CancellationToken ct = default)
        {
            var item = await _db.Items
                .AsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new ProductListItemDto(
                    i.Id,
                    i.Name,
                    i.Price,
                    i.Material,
                    i.Category,
                    i.AffiliateUrl,
                    i.IsVipOnly,
                    i.IsActive,
                    i.CreatedAt,
                    i.ImageBase64 != null ? $"data:image/jpeg;base64,{i.ImageBase64}" : null
                ))
                .FirstOrDefaultAsync(ct);

            if (item == null) return NotFound();
            return Ok(item);
        }

        // =====================================================
        // POST /api/ManagerProduct
        // =====================================================
        [HttpPost]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Tạo mới sản phẩm (multipart/form-data)")]
        public async Task<IActionResult> Create([FromForm] ItemCreateDto dto, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                string? base64Image = null;
                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    using var ms = new MemoryStream();
                    await dto.ImageFile.CopyToAsync(ms, ct);
                    base64Image = Convert.ToBase64String(ms.ToArray());
                }

                // ✅ Cho phép nhập Material/Category mới hoặc chọn sẵn
                var item = new Item
                {
                    Name = string.IsNullOrWhiteSpace(dto.Name) ? "(No Name)" : dto.Name.Trim(),
                    Price = dto.Price,
                    Material = string.IsNullOrWhiteSpace(dto.Material) ? null : dto.Material.Trim(),
                    Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim(),
                    AffiliateUrl = dto.AffiliateUrl,
                    ImageBase64 = base64Image,
                    IsVipOnly = dto.IsVipOnly,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Items.Add(item);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                var result = new ProductListItemDto(
                    item.Id,
                    item.Name,
                    item.Price,
                    item.Material,
                    item.Category,
                    item.AffiliateUrl,
                    item.IsVipOnly,
                    item.IsActive,
                    item.CreatedAt,
                    base64Image != null ? $"data:image/jpeg;base64,{base64Image}" : null
                );

                return CreatedAtAction(nameof(GetById), new { id = item.Id }, result);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                return StatusCode(500, new { message = "Create item failed", error = ex.Message });
            }
        }

        // =====================================================
        // GET /api/ManagerProduct/filters
        // =====================================================
        [HttpGet("filters")]
        [SwaggerOperation(Summary = "Lấy danh sách Material & Category (dropdown)")]
        public async Task<IActionResult> GetFilters(CancellationToken ct)
        {
            var materials = await _db.Items.AsNoTracking()
                .Where(i => !string.IsNullOrWhiteSpace(i.Material))
                .Select(i => i.Material!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(ct);

            var categories = await _db.Items.AsNoTracking()
                .Where(i => !string.IsNullOrWhiteSpace(i.Category))
                .Select(i => i.Category!.Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync(ct);

            return Ok(new { materials, categories });
        }

        // =====================================================
        // GET /api/ManagerProduct
        // =====================================================
        [HttpGet]
        [SwaggerOperation(Summary = "Danh sách sản phẩm (search, filter, paging, thống kê)")]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? material,
            [FromQuery] string? category,
            [FromQuery] bool? isVipOnly,
            [FromQuery] bool? isActive,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            CancellationToken ct = default)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 12;
            if (pageSize > 100) pageSize = 100;

            var q = _db.Items.AsNoTracking().AsQueryable();

            // 🔍 Search & Filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                var kw = search.Trim().ToLower();
                q = q.Where(i =>
                    i.Name.ToLower().Contains(kw) ||
                    (i.Material != null && i.Material.ToLower().Contains(kw)) ||
                    (i.Category != null && i.Category.ToLower().Contains(kw)));
            }

            if (!string.IsNullOrWhiteSpace(material))
            {
                var mat = material.Trim().ToLower();
                q = q.Where(i => i.Material != null && i.Material.ToLower() == mat);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                var cat = category.Trim().ToLower();
                q = q.Where(i => i.Category != null && i.Category.ToLower() == cat);
            }

            if (isVipOnly.HasValue) q = q.Where(i => i.IsVipOnly == isVipOnly.Value);
            if (isActive.HasValue) q = q.Where(i => i.IsActive == isActive.Value);

            var totalCount = await q.CountAsync(ct);

            var items = await q
                .OrderByDescending(i => i.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new ProductListItemDto(
                    i.Id,
                    i.Name,
                    i.Price,
                    i.Material,
                    i.Category,
                    i.AffiliateUrl,
                    i.IsVipOnly,
                    i.IsActive,
                    i.CreatedAt,
                    i.ImageBase64 != null ? $"data:image/jpeg;base64,{i.ImageBase64}" : null
                ))
                .ToListAsync(ct);

            var totalItems = await _db.Items.CountAsync(ct);
            var activeCount = await _db.Items.CountAsync(i => i.IsActive, ct);
            var inactiveCount = totalItems - activeCount;
            var vipCount = await _db.Items.CountAsync(i => i.IsVipOnly, ct);
            var normalCount = totalItems - vipCount;

            var byCategory = await _db.Items
                .GroupBy(i => string.IsNullOrEmpty(i.Category) ? "(Chưa gán)" : i.Category!)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync(ct);

            var byMaterial = await _db.Items
                .GroupBy(i => string.IsNullOrEmpty(i.Material) ? "(Chưa gán)" : i.Material!)
                .Select(g => new { Material = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync(ct);

            return Ok(new
            {
                totalCount,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                statusSummary = new
                {
                    totalItems,
                    activeCount,
                    inactiveCount,
                    vipCount,
                    normalCount,
                    byCategory,
                    byMaterial
                },
                data = items
            });
        }

        // =====================================================
        // PUT /api/ManagerProduct/{id}
        // =====================================================
        public class ItemUpdateDto
        {
            public string? Name { get; set; }
            public decimal? Price { get; set; }
            public bool? ClearPrice { get; set; }
            public string? Material { get; set; }
            public string? Category { get; set; }
            public string? AffiliateUrl { get; set; }
            public bool? IsVipOnly { get; set; }
            public bool? IsActive { get; set; }

            public IFormFile? ImageFile { get; set; }
            public bool? RemoveImage { get; set; }
        }

        [HttpPut("{id:long}")]
        [Consumes("multipart/form-data")]
        [SwaggerOperation(Summary = "Cập nhật sản phẩm (multipart/form-data)")]
        public async Task<IActionResult> Update(long id, [FromForm] ItemUpdateDto dto, CancellationToken ct = default)
        {
            var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (item == null) return NotFound(new { message = "Item not found" });

            if (dto.Name != null && !string.IsNullOrWhiteSpace(dto.Name))
                item.Name = dto.Name.Trim();

            if (dto.ClearPrice == true) item.Price = null;
            else if (dto.Price.HasValue) item.Price = dto.Price;

            // ✅ Cho phép cập nhật hoặc thêm Material/Category mới
            if (dto.Material != null)
                item.Material = string.IsNullOrWhiteSpace(dto.Material) ? null : dto.Material.Trim();

            if (dto.Category != null)
                item.Category = string.IsNullOrWhiteSpace(dto.Category) ? null : dto.Category.Trim();

            if (dto.AffiliateUrl != null)
                item.AffiliateUrl = dto.AffiliateUrl;

            if (dto.IsVipOnly.HasValue)
                item.IsVipOnly = dto.IsVipOnly.Value;

            if (dto.IsActive.HasValue)
                item.IsActive = dto.IsActive.Value;

            if (dto.RemoveImage == true)
            {
                item.ImageBase64 = null;
            }
            else if (dto.ImageFile != null && dto.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await dto.ImageFile.CopyToAsync(ms, ct);
                item.ImageBase64 = Convert.ToBase64String(ms.ToArray());
            }

            await _db.SaveChangesAsync(ct);

            var result = new ProductListItemDto(
                item.Id,
                item.Name,
                item.Price,
                item.Material,
                item.Category,
                item.AffiliateUrl,
                item.IsVipOnly,
                item.IsActive,
                item.CreatedAt,
                item.ImageBase64 != null ? $"data:image/jpeg;base64,{item.ImageBase64}" : null
            );

            return Ok(result);
        }

        // =====================================================
        // DELETE /api/ManagerProduct/{id} (Soft Delete)
        // =====================================================
        [HttpDelete("{id:long}")]
        [SwaggerOperation(Summary = "Vô hiệu hoá (soft delete) sản phẩm bằng IsActive=false")]
        public async Task<IActionResult> SoftDelete(long id, CancellationToken ct = default)
        {
            var item = await _db.Items.FirstOrDefaultAsync(i => i.Id == id, ct);
            if (item == null) return NotFound(new { message = "Item not found" });

            if (!item.IsActive)
                return Ok(new { ok = true, id, status = "already_inactive" });

            item.IsActive = false;
            await _db.SaveChangesAsync(ct);
            return Ok(new { ok = true, id, status = "deactivated" });
        }

        // =====================================================
        // DELETE /api/ManagerProduct/{id}/hard (Hard Delete)
        // =====================================================
        [HttpDelete("{id:long}/hard")]
        [SwaggerOperation(Summary = "Xoá cứng sản phẩm và liên kết phụ thuộc (transaction safe)")]
        public async Task<IActionResult> HardDelete(long id, CancellationToken ct = default)
        {
            await using var tx = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var item = await _db.Items
                    .Include(i => i.AffiliateLinks)
                    .Include(i => i.ClosetItems)
                    .Include(i => i.ItemAttributes)
                    .Include(i => i.ItemCategories)
                    .Include(i => i.Outfits)
                    .FirstOrDefaultAsync(i => i.Id == id, ct);

                if (item == null) return NotFound(new { message = "Item not found" });

                _db.AffiliateLinks.RemoveRange(item.AffiliateLinks);
                _db.ClosetItems.RemoveRange(item.ClosetItems);
                _db.ItemAttributes.RemoveRange(item.ItemAttributes);
                _db.ItemCategories.RemoveRange(item.ItemCategories);
                _db.Outfits.RemoveRange(item.Outfits);

                _db.Items.Remove(item);
                await _db.SaveChangesAsync(ct);
                await tx.CommitAsync(ct);

                return Ok(new { ok = true, id });
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync(ct);
                return Conflict(new
                {
                    message = "Cannot hard delete due to related records. Try soft delete instead.",
                    error = ex.Message
                });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync(ct);
                return StatusCode(500, new { message = "Hard delete failed", error = ex.Message });
            }
        }
    }
}

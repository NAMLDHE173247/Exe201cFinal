using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Images;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Infrastructure.Services;

public sealed class ImageReadService : IImageReadService
{
    private readonly TryOnDbContext _db;
    public ImageReadService(TryOnDbContext db) => _db = db;

    public async Task<(string ContentType, byte[] Bytes)?> GetAsync(long imageId, CancellationToken ct = default)
    {
        var img = await _db.Images.AsNoTracking().FirstOrDefaultAsync(x => x.Id == imageId, ct);
        if (img == null) return null;
        if (img.StorageType != 0 || img.Data == null) return null; // chỉ hỗ trợ DbBlob ở MVP
        return (img.ContentType ?? "application/octet-stream", img.Data);
    }
}

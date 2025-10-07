using Microsoft.EntityFrameworkCore;
using StyleGenie.Application.Dto.TryOn;
using StyleGenie.Application.TryOn;
using StyleGenie.Application.Wallet;
using StyleGenie.Infrastructure.Data.Models;

namespace StyleGenie.Infrastructure.Services;

public sealed class TryOnService : ITryOnService
{
    private readonly TryOnDbContext _db;
    private readonly ITryOnProvider _provider;
    private readonly IWalletService _wallet;

    public TryOnService(TryOnDbContext db, ITryOnProvider provider, IWalletService wallet)
    {
        _db = db;
        _provider = provider;
        _wallet = wallet;
    }

    public async Task<TryOnResultDto> TryOnAsync(TryOnRequestDto req, CancellationToken ct = default)
    {
        // Kiểm tra và trừ credit
        if (!await _wallet.TryDebitAsync(req.TenantId, 1, "TRYON", null, null))
            throw new InvalidOperationException("Insufficient credits");

        var model = new Image { TenantId = req.TenantId, Kind = 0, ContentType = "image/jpeg", StorageType = 0, Data = req.ModelJpeg, CreatedAt = DateTime.UtcNow };
        var cloth = new Image { TenantId = req.TenantId, Kind = 1, ContentType = "image/jpeg", StorageType = 0, Data = req.ClothJpeg, CreatedAt = DateTime.UtcNow };
        _db.Images.AddRange(model, cloth);

        Image? lower = null;
        if (req.ClothType == "combo" && req.LowerClothJpeg is not null)
        {
            lower = new Image { TenantId = req.TenantId, Kind = 2, ContentType = "image/jpeg", StorageType = 0, Data = req.LowerClothJpeg, CreatedAt = DateTime.UtcNow };
            _db.Images.Add(lower);
        }
        await _db.SaveChangesAsync(ct);

        var job = new TryOnJob
        {
            TenantId = req.TenantId,
            Provider = "FitRoom",
            ClothType = req.ClothType,
            HdMode = req.HdMode,
            Status = "CREATED",
            Progress = 0,
            ModelImageId = model.Id,
            ClothImageId = cloth.Id,
            LowerClothImageId = lower?.Id,
            CreatedAt = DateTime.UtcNow
        };
        _db.TryOnJobs.Add(job);
        await _db.SaveChangesAsync(ct);

        try
        {
            var (taskId, downloader) = await _provider.CreateAndPollAsync(
                req.TenantId, req.ClothType, req.HdMode,
                req.ModelJpeg, req.ClothJpeg, req.LowerClothJpeg, ct);

            job.ProviderTaskId = taskId;
            job.Status = "PROCESSING";
            await _db.SaveChangesAsync(ct);

            var bytes = await downloader(ct);

            var result = new Image
            {
                TenantId = req.TenantId,
                Kind = 3,
                ContentType = "image/jpeg",
                StorageType = 0,
                Data = bytes,
                CreatedAt = DateTime.UtcNow
            };
            _db.Images.Add(result);
            await _db.SaveChangesAsync(ct);

            job.ResultImageId = result.Id;
            job.Status = "COMPLETED";
            job.Progress = 100;
            job.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            return new TryOnResultDto(job.Id, bytes);
        }
        catch (Exception ex)
        {
            job.Status = "FAILED";
            job.Error = ex.Message;
            job.CompletedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            await _wallet.CreditAsync(req.TenantId, 1, "ADJUST", "rollback on failure");

            if (ex is UnauthorizedAccessException)
                throw; // bubble lên Controller

            throw;
        }
    }
}

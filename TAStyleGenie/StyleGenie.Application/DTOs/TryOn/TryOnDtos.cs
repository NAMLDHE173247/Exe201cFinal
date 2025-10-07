namespace StyleGenie.Application.Dto.TryOn;

// ✅ DTO cho request từ frontend (không cần TenantId)
public record TryOnRequestDto(long TenantId,
    string ClothType, bool HdMode,
    byte[] ModelJpeg, byte[] ClothJpeg, byte[]? LowerClothJpeg);

// ✅ DTO cho service internal (có TenantId)
public record TryOnServiceDto(
    long TenantId, string ClothType, bool HdMode,
    byte[] ModelJpeg, byte[] ClothJpeg, byte[]? LowerClothJpeg);

public record TryOnResultDto(long JobId, byte[] ResultJpeg);

namespace StyleGenie.Application.Dto.Items;

public record ItemDetailDto(
    long Id,
    string Name,
    long? TenantId,
    long? PrimaryImageId,
    string? Color,
    string? Size,
    string? Material,
    string? AttributesJson,
    bool IsVipOnly,
    bool IsActive,
    long? CreatedBy,
    DateTime CreatedAt);
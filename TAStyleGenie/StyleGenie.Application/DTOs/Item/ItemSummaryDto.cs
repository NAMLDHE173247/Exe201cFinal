namespace StyleGenie.Application.Dto.Items;

public record ItemSummaryDto(
    long Id,
    string Name,
    long? TenantId,
    bool IsVipOnly,
    bool IsActive,
    DateTime CreatedAt);
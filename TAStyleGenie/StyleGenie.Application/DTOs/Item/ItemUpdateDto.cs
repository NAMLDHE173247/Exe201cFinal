namespace StyleGenie.Application.Dto.Items;

public class ItemUpdateDto
{
    public required string Name { get; init; }
    public long? TenantId { get; init; }
    public long? PrimaryImageId { get; init; }
    public string? Color { get; init; }
    public string? Size { get; init; }
    public string? Material { get; init; }
    public string? AttributesJson { get; init; }
    public bool IsVipOnly { get; init; }
    public bool IsActive { get; init; }
    public long? CreatedBy { get; init; }
}
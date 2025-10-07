namespace StyleGenie.Application.Dto.Item
{
    public class AffiliateLinkDto
    {
        public long Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public decimal? CommissionRate { get; set; }
        public int ClickCount { get; set; }
        public int ConversionCount { get; set; }
    }

    public class ImageDto
    {
        public long Id { get; set; }
        public string? Url { get; set; }
        public string? ContentType { get; set; }
    }

    public class ItemDetailDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public bool IsVipOnly { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        public ImageDto? PrimaryImage { get; set; }
        public List<AffiliateLinkDto> AffiliateLinks { get; set; } = new();
    }

    // DTO để tạo mới Item (kèm link + ảnh ID có sẵn)
    public class ItemCreateDto
    {
        public long? TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public bool IsVipOnly { get; set; }
        public bool IsActive { get; set; } = true;
        public long? CreatedBy { get; set; }

        // Id ảnh đã upload sẵn (Images)
        public long? PrimaryImageId { get; set; }

        // Affiliate link
        public string? AffiliateUrl { get; set; }
        public decimal? CommissionRate { get; set; }
    }

    // DTO để update Item
    public class ItemUpdateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public string? Material { get; set; }
        public bool IsVipOnly { get; set; }
        public bool IsActive { get; set; }
        public long? PrimaryImageId { get; set; }

        public string? AffiliateUrl { get; set; }
        public decimal? CommissionRate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models
{
    public partial class Item
    {
        [Key]
        public long Id { get; set; }

        public long? TenantId { get; set; }

        [StringLength(256)]
        public string Name { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        public long? PrimaryImageId { get; set; }

        [StringLength(64)]
        public string? Color { get; set; }

        [StringLength(64)]
        public string? Size { get; set; }

        [StringLength(128)]
        public string? Material { get; set; }
        public string? Category { get; set; }

        public string? AttributesJson { get; set; }

        public bool IsVipOnly { get; set; }

        public bool IsActive { get; set; }

        public long? CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        // ✅ Ảnh dạng Base64 (text)
        [Column(TypeName = "nvarchar(max)")]
        public string? ImageBase64 { get; set; }

        // ✅ Link Affiliate lưu trực tiếp
        [StringLength(512)]
        public string? AffiliateUrl { get; set; }

        // ===== Giữ nguyên các quan hệ cũ (để không lỗi context) =====
        [InverseProperty("Item")]
        public virtual ICollection<AffiliateLink> AffiliateLinks { get; set; } = new List<AffiliateLink>();

        [InverseProperty("Item")]
        public virtual ICollection<ClosetItem> ClosetItems { get; set; } = new List<ClosetItem>();

        [ForeignKey("CreatedBy")]
        [InverseProperty("Items")]
        public virtual User? CreatedByNavigation { get; set; }

        [InverseProperty("Item")]
        public virtual ICollection<ItemAttribute> ItemAttributes { get; set; } = new List<ItemAttribute>();

        [InverseProperty("Item")]
        public virtual ICollection<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();

        [ForeignKey("PrimaryImageId")]
        [InverseProperty("Items")]
        public virtual Image? PrimaryImage { get; set; }

        [ForeignKey("TenantId")]
        [InverseProperty("Items")]
        public virtual Tenant? Tenant { get; set; }

        [ForeignKey("ItemId")]
        [InverseProperty("Items")]
        public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
    }
}

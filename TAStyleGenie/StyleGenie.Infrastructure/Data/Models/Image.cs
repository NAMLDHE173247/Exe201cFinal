using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("TenantId", "Kind", Name = "IX_Images_TenantId_Kind")]
public partial class Image
{
    [Key]
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public byte Kind { get; set; }

    [StringLength(64)]
    public string ContentType { get; set; } = null!;

    public byte StorageType { get; set; }

    public byte[]? Data { get; set; }

    [StringLength(1024)]
    public string? Url { get; set; }

    [StringLength(64)]
    [Unicode(false)]
    public string? Sha256Hex { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Image")]
    public virtual ICollection<Banner> Banners { get; set; } = new List<Banner>();

    [InverseProperty("PrimaryImage")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("ResultImage")]
    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();

    [ForeignKey("TenantId")]
    [InverseProperty("Images")]
    public virtual Tenant? Tenant { get; set; }

    [InverseProperty("ClothImage")]
    public virtual ICollection<TryOnJob> TryOnJobClothImages { get; set; } = new List<TryOnJob>();

    [InverseProperty("LowerClothImage")]
    public virtual ICollection<TryOnJob> TryOnJobLowerClothImages { get; set; } = new List<TryOnJob>();

    [InverseProperty("ModelImage")]
    public virtual ICollection<TryOnJob> TryOnJobModelImages { get; set; } = new List<TryOnJob>();

    [InverseProperty("ResultImage")]
    public virtual ICollection<TryOnJob> TryOnJobResultImages { get; set; } = new List<TryOnJob>();

    [InverseProperty("AvatarImage")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

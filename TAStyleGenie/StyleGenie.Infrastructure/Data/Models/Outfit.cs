using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Outfit
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? MannequinId { get; set; }

    public long? LayoutId { get; set; }

    public long? ResultImageId { get; set; }

    public bool IsPublic { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Outfit")]
    public virtual ICollection<AIStylingHistory> AIStylingHistories { get; set; } = new List<AIStylingHistory>();

    [InverseProperty("Outfit")]
    public virtual ICollection<ClosetOutfitHistory> ClosetOutfitHistories { get; set; } = new List<ClosetOutfitHistory>();

    [ForeignKey("LayoutId")]
    [InverseProperty("Outfits")]
    public virtual Layout? Layout { get; set; }

    [ForeignKey("MannequinId")]
    [InverseProperty("Outfits")]
    public virtual Mannequin? Mannequin { get; set; }

    [ForeignKey("ResultImageId")]
    [InverseProperty("Outfits")]
    public virtual Image? ResultImage { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Outfits")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("OutfitId")]
    [InverseProperty("Outfits")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();
}

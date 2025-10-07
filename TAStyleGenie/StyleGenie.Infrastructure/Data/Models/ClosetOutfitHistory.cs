using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("ClosetOutfitHistory")]
public partial class ClosetOutfitHistory
{
    [Key]
    public long Id { get; set; }

    public long ClosetId { get; set; }

    public long OutfitId { get; set; }

    public string? SavedImages { get; set; }

    public DateTime SavedAt { get; set; }

    [ForeignKey("ClosetId")]
    [InverseProperty("ClosetOutfitHistories")]
    public virtual Closet Closet { get; set; } = null!;

    [ForeignKey("OutfitId")]
    [InverseProperty("ClosetOutfitHistories")]
    public virtual Outfit Outfit { get; set; } = null!;
}

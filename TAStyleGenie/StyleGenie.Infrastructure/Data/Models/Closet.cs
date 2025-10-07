using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Closet
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Closet")]
    public virtual ICollection<ClosetCategory> ClosetCategories { get; set; } = new List<ClosetCategory>();

    [InverseProperty("Closet")]
    public virtual ICollection<ClosetItem> ClosetItems { get; set; } = new List<ClosetItem>();

    [InverseProperty("Closet")]
    public virtual ICollection<ClosetOutfitHistory> ClosetOutfitHistories { get; set; } = new List<ClosetOutfitHistory>();

    [ForeignKey("UserId")]
    [InverseProperty("Closets")]
    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Mannequin
{
    [Key]
    public long Id { get; set; }

    [StringLength(64)]
    public string Name { get; set; } = null!;

    [StringLength(16)]
    public string? Gender { get; set; }

    [StringLength(64)]
    public string? BodyType { get; set; }

    [Column(TypeName = "decimal(6, 2)")]
    public decimal? HeightCm { get; set; }

    [StringLength(64)]
    public string? SkinTone { get; set; }

    public bool IsVipOnly { get; set; }

    public bool IsCustom { get; set; }

    public long? CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Mannequins")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Mannequin")]
    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}

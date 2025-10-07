using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Layout
{
    [Key]
    public long Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    [StringLength(512)]
    public string? Description { get; set; }

    public bool IsVipOnly { get; set; }

    public long? CreatedBy { get; set; }

    [ForeignKey("CreatedBy")]
    [InverseProperty("Layouts")]
    public virtual User? CreatedByNavigation { get; set; }

    [InverseProperty("Layout")]
    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();
}

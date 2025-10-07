using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class PromotionPlan
{
    [Key]
    public long Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    [StringLength(512)]
    public string? Description { get; set; }

    public int? DiscountPercent { get; set; }

    public int? ExtraDays { get; set; }

    public DateOnly? ValidFrom { get; set; }

    public DateOnly? ValidTo { get; set; }

    public bool IsActive { get; set; }

    [InverseProperty("Promotion")]
    public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();
}

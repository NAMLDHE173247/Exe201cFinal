using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class SubscriptionPlan
{
    [Key]
    public long Id { get; set; }

    [StringLength(64)]
    public string Name { get; set; } = null!;

    public int? MaxItemsPerDay { get; set; }

    public int? MaxLayoutsPerDay { get; set; }

    public int? MannequinLimit { get; set; }

    public bool HasWatermark { get; set; }

    public bool HasAds { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? Price { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    [InverseProperty("SubscriptionPlan")]
    public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();

    [InverseProperty("SubscriptionPlan")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    [InverseProperty("Plan")]
    public virtual ICollection<VipPayment> VipPayments { get; set; } = new List<VipPayment>();
}

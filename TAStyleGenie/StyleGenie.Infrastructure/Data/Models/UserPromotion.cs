using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class UserPromotion
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PromotionId { get; set; }

    public long? SubscriptionPlanId { get; set; }

    public DateTime AppliedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    [StringLength(256)]
    public string? Note { get; set; }

    [ForeignKey("PromotionId")]
    [InverseProperty("UserPromotions")]
    public virtual PromotionPlan Promotion { get; set; } = null!;

    [ForeignKey("SubscriptionPlanId")]
    [InverseProperty("UserPromotions")]
    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserPromotions")]
    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Transaction
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(32)]
    public string Type { get; set; } = null!;

    [Column(TypeName = "decimal(12, 2)")]
    public decimal Amount { get; set; }

    [StringLength(32)]
    public string Status { get; set; } = null!;

    [StringLength(512)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? PayOspaymentLinkId { get; set; }

    public string? PayOscheckoutUrl { get; set; }

    public string? PayOsresponseJson { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public long? PlanId { get; set; }

    public virtual SubscriptionPlan? Plan { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Transactions")]
    public virtual User User { get; set; } = null!;

    [InverseProperty("Transaction")]
    public virtual ICollection<VipPayment> VipPayments { get; set; } = new List<VipPayment>();
}

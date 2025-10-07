using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class VipPayment
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PlanId { get; set; }

    public long? TransactionId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    [ForeignKey("PlanId")]
    [InverseProperty("VipPayments")]
    public virtual SubscriptionPlan Plan { get; set; } = null!;

    [ForeignKey("TransactionId")]
    [InverseProperty("VipPayments")]
    public virtual Transaction? Transaction { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("VipPayments")]
    public virtual User User { get; set; } = null!;
}

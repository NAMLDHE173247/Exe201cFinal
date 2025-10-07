using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("UsageLedger")]
[Index("TenantId", "CreatedAt", Name = "IX_UsageLedger_TenantId_CreatedAt", IsDescending = new[] { false, true })]
public partial class UsageLedger
{
    [Key]
    public long Id { get; set; }

    public long TenantId { get; set; }

    public long? JobId { get; set; }

    public int Credits { get; set; }

    [StringLength(64)]
    public string Reason { get; set; } = null!;

    [StringLength(256)]
    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("JobId")]
    [InverseProperty("UsageLedgers")]
    public virtual TryOnJob? Job { get; set; }

    [ForeignKey("TenantId")]
    [InverseProperty("UsageLedgers")]
    public virtual Tenant Tenant { get; set; } = null!;
}

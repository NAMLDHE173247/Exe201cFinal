using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("TenantId", "CreatedAt", Name = "IX_TryOnJobs_TenantId_CreatedAt", IsDescending = new[] { false, true })]
public partial class TryOnJob
{
    [Key]
    public long Id { get; set; }

    public long TenantId { get; set; }

    [StringLength(32)]
    public string Provider { get; set; } = null!;

    [StringLength(16)]
    public string ClothType { get; set; } = null!;

    public bool HdMode { get; set; }

    [StringLength(16)]
    public string Status { get; set; } = null!;

    [StringLength(128)]
    public string? ProviderTaskId { get; set; }

    public int? Progress { get; set; }

    public long ModelImageId { get; set; }

    public long? ClothImageId { get; set; }

    public long? LowerClothImageId { get; set; }

    public long? ResultImageId { get; set; }

    [StringLength(512)]
    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    [ForeignKey("ClothImageId")]
    [InverseProperty("TryOnJobClothImages")]
    public virtual Image? ClothImage { get; set; }

    [ForeignKey("LowerClothImageId")]
    [InverseProperty("TryOnJobLowerClothImages")]
    public virtual Image? LowerClothImage { get; set; }

    [ForeignKey("ModelImageId")]
    [InverseProperty("TryOnJobModelImages")]
    public virtual Image ModelImage { get; set; } = null!;

    [ForeignKey("ResultImageId")]
    [InverseProperty("TryOnJobResultImages")]
    public virtual Image? ResultImage { get; set; }

    [ForeignKey("TenantId")]
    [InverseProperty("TryOnJobs")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("Job")]
    public virtual ICollection<UsageLedger> UsageLedgers { get; set; } = new List<UsageLedger>();
}

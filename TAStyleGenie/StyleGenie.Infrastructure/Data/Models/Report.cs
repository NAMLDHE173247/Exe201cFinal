using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Report
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(64)]
    public string Type { get; set; } = null!;

    [StringLength(1024)]
    public string? Content { get; set; }

    [StringLength(64)]
    public string? TargetType { get; set; }

    public long? TargetId { get; set; }

    [StringLength(32)]
    public string Status { get; set; } = null!;

    public long? HandledBy { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("HandledBy")]
    [InverseProperty("ReportHandledByNavigations")]
    public virtual User? HandledByNavigation { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ReportUsers")]
    public virtual User User { get; set; } = null!;
}

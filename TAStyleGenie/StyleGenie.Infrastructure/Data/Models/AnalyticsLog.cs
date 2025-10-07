using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class AnalyticsLog
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    [StringLength(64)]
    public string ActionType { get; set; } = null!;

    [StringLength(64)]
    public string? TargetType { get; set; }

    public long? TargetId { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AnalyticsLogs")]
    public virtual User? User { get; set; }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("SkinToneAnalysisLog")]
public partial class SkinToneAnalysisLog
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    [StringLength(64)]
    public string? DetectedSkinTone { get; set; }

    public string? AnalysisResult { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("SkinToneAnalysisLogs")]
    public virtual User User { get; set; } = null!;
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("AdsClickLog")]
public partial class AdsClickLog
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long BannerId { get; set; }

    public DateTime ClickedAt { get; set; }

    [StringLength(1024)]
    public string DestinationUrl { get; set; } = null!;

    [ForeignKey("BannerId")]
    [InverseProperty("AdsClickLogs")]
    public virtual Banner Banner { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AdsClickLogs")]
    public virtual User? User { get; set; }
}

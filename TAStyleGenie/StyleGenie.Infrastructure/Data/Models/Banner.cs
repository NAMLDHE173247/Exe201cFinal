using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Banner
{
    [Key]
    public long Id { get; set; }

    public long? ImageId { get; set; }

    [StringLength(1024)]
    public string DestinationUrl { get; set; } = null!;

    public bool Active { get; set; }

    [StringLength(64)]
    public string? DisplayPosition { get; set; }

    [InverseProperty("Banner")]
    public virtual ICollection<AdsClickLog> AdsClickLogs { get; set; } = new List<AdsClickLog>();

    [ForeignKey("ImageId")]
    [InverseProperty("Banners")]
    public virtual Image? Image { get; set; }
}

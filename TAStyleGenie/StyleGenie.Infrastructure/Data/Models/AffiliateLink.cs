using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class AffiliateLink
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    public long ItemId { get; set; }

    [StringLength(1024)]
    public string Url { get; set; } = null!;

    public int ClickCount { get; set; }

    public int ConversionCount { get; set; }

    [Column(TypeName = "decimal(6, 4)")]
    public decimal? CommissionRate { get; set; }

    [ForeignKey("ItemId")]
    [InverseProperty("AffiliateLinks")]
    public virtual Item Item { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("AffiliateLinks")]
    public virtual User? User { get; set; }
}

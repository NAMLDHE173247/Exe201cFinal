using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("AIStylingHistory")]
public partial class AIStylingHistory
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long? OutfitId { get; set; }

    public string? FilterCriteria { get; set; }

    public bool ClickedAffiliate { get; set; }

    public bool SavedToCloset { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("OutfitId")]
    [InverseProperty("AIStylingHistories")]
    public virtual Outfit? Outfit { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("AIStylingHistories")]
    public virtual User User { get; set; } = null!;
}

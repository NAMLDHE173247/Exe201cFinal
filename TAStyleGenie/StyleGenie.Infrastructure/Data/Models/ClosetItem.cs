using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class ClosetItem
{
    [Key]
    public long Id { get; set; }

    public long ClosetId { get; set; }

    public long ItemId { get; set; }

    public long? CategoryId { get; set; }

    [StringLength(256)]
    public string? Note { get; set; }

    public DateTime AddedAt { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ClosetItems")]
    public virtual ClosetCategory? Category { get; set; }

    [ForeignKey("ClosetId")]
    [InverseProperty("ClosetItems")]
    public virtual Closet Closet { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ClosetItems")]
    public virtual Item Item { get; set; } = null!;
}

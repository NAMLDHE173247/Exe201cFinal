using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class ClosetCategory
{
    [Key]
    public long Id { get; set; }

    public long ClosetId { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("ClosetId")]
    [InverseProperty("ClosetCategories")]
    public virtual Closet Closet { get; set; } = null!;

    [InverseProperty("Category")]
    public virtual ICollection<ClosetItem> ClosetItems { get; set; } = new List<ClosetItem>();
}

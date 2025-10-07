using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("ItemId", "CategoryId", Name = "UX_ItemCategories", IsUnique = true)]
public partial class ItemCategory
{
    [Key]
    public long Id { get; set; }

    public long ItemId { get; set; }

    public long CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("ItemCategories")]
    public virtual Category Category { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemCategories")]
    public virtual Item Item { get; set; } = null!;
}

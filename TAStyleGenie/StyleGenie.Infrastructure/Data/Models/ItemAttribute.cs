using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("ItemId", "AttributeValueId", Name = "UX_ItemAttributes", IsUnique = true)]
public partial class ItemAttribute
{
    [Key]
    public long Id { get; set; }

    public long ItemId { get; set; }

    public long AttributeValueId { get; set; }

    [ForeignKey("AttributeValueId")]
    [InverseProperty("ItemAttributes")]
    public virtual AttributeValue AttributeValue { get; set; } = null!;

    [ForeignKey("ItemId")]
    [InverseProperty("ItemAttributes")]
    public virtual Item Item { get; set; } = null!;
}

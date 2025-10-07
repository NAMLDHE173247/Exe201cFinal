using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class AttributeValue
{
    [Key]
    public long Id { get; set; }

    public long AttributeTypeId { get; set; }

    [StringLength(128)]
    public string Value { get; set; } = null!;

    [StringLength(7)]
    [Unicode(false)]
    public string? HexColor { get; set; }

    [ForeignKey("AttributeTypeId")]
    [InverseProperty("AttributeValues")]
    public virtual AttributeType AttributeType { get; set; } = null!;

    [InverseProperty("AttributeValue")]
    public virtual ICollection<ItemAttribute> ItemAttributes { get; set; } = new List<ItemAttribute>();
}

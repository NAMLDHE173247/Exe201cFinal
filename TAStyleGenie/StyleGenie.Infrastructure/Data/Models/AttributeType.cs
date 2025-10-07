using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class AttributeType
{
    [Key]
    public long Id { get; set; }

    [StringLength(64)]
    public string Name { get; set; } = null!;

    [StringLength(256)]
    public string? Description { get; set; }

    [InverseProperty("AttributeType")]
    public virtual ICollection<AttributeValue> AttributeValues { get; set; } = new List<AttributeValue>();
}

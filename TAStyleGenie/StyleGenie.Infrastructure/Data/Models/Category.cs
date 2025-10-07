using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Category
{
    [Key]
    public long Id { get; set; }

    public long? TenantId { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    public long? ParentId { get; set; }

    [StringLength(512)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    [InverseProperty("Category")]
    public virtual ICollection<ItemCategory> ItemCategories { get; set; } = new List<ItemCategory>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Category? Parent { get; set; }

    [ForeignKey("TenantId")]
    [InverseProperty("Categories")]
    public virtual Tenant? Tenant { get; set; }
}

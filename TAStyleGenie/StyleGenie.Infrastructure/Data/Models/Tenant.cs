using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class Tenant
{
    [Key]
    public long Id { get; set; }

    [StringLength(128)]
    public string Name { get; set; } = null!;

    public byte Status { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Tenant")]
    public virtual ICollection<ApiCredential> ApiCredentials { get; set; } = new List<ApiCredential>();

    [InverseProperty("Tenant")]
    public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

    [InverseProperty("Tenant")]
    public virtual CreditWallet? CreditWallet { get; set; }

    [InverseProperty("Tenant")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("Tenant")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("Tenant")]
    public virtual ICollection<TryOnJob> TryOnJobs { get; set; } = new List<TryOnJob>();

    [InverseProperty("Tenant")]
    public virtual ICollection<UsageLedger> UsageLedgers { get; set; } = new List<UsageLedger>();

    [InverseProperty("Tenant")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}

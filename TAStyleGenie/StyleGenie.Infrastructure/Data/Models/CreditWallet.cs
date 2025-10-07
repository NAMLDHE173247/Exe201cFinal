using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class CreditWallet
{
    [Key]
    public long TenantId { get; set; }

    public int Balance { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[] RowVer { get; set; } = null!;

    [ForeignKey("TenantId")]
    [InverseProperty("CreditWallet")]
    public virtual Tenant Tenant { get; set; } = null!;
}

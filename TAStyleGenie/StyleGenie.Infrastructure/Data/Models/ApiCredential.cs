using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("TenantId", "Provider", "KeyName", Name = "UX_ApiCredentials", IsUnique = true)]
public partial class ApiCredential
{
    [Key]
    public long Id { get; set; }

    public long? TenantId { get; set; }

    [StringLength(64)]
    public string Provider { get; set; } = null!;

    [StringLength(64)]
    public string KeyName { get; set; } = null!;

    public string SecretCipher { get; set; } = null!;

    public DateTime UpdatedAt { get; set; }

    [StringLength(128)]
    public string? UpdatedBy { get; set; }

    [ForeignKey("TenantId")]
    [InverseProperty("ApiCredentials")]
    public virtual Tenant? Tenant { get; set; }
}

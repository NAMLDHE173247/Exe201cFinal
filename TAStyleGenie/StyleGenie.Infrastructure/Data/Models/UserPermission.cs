using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("UserId", "PermissionId", Name = "UX_UserPermissions", IsUnique = true)]
public partial class UserPermission
{
    [Key]
    public long Id { get; set; }

    public long UserId { get; set; }

    public long PermissionId { get; set; }

    [ForeignKey("PermissionId")]
    [InverseProperty("UserPermissions")]
    public virtual Permission Permission { get; set; } = null!;

    [ForeignKey("UserId")]
    [InverseProperty("UserPermissions")]
    public virtual User User { get; set; } = null!;
}

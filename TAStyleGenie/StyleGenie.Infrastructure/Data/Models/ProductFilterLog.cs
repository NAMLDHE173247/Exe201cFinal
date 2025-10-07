using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class ProductFilterLog
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    public string? FilterCriteria { get; set; }

    public int? ResultCount { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ProductFilterLogs")]
    public virtual User? User { get; set; }
}

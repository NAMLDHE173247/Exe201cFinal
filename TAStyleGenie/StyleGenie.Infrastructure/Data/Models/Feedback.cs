using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    [StringLength(64)]
    public string? FeedbackType { get; set; }

    public int? Rating { get; set; }

    [StringLength(1024)]
    public string? Message { get; set; }

    public DateTime CreatedAt { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Feedbacks")]
    public virtual User? User { get; set; }
}

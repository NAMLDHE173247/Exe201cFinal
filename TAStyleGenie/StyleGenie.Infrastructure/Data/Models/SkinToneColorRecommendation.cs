using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class SkinToneColorRecommendation
{
    [Key]
    public long Id { get; set; }

    [StringLength(64)]
    public string SkinTone { get; set; } = null!;

    [StringLength(64)]
    public string ColorName { get; set; } = null!;

    [StringLength(7)]
    [Unicode(false)]
    public string HexCode { get; set; } = null!;
}

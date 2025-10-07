using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

[Index("Email", Name = "UX_Users_Email", IsUnique = true)]
public partial class User
{
    [Key]
    public long Id { get; set; }

    public long TenantId { get; set; }

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(256)]
    public string? PasswordHash { get; set; }

    [StringLength(128)]
    public string? FullName { get; set; }

    public long? AvatarImageId { get; set; }

    public bool IsVerified { get; set; }

    public long? RoleId { get; set; }

    public long? SubscriptionPlanId { get; set; }

    public DateOnly? Dob { get; set; }

    [StringLength(32)]
    public string? Phone { get; set; }

    public bool IsActive { get; set; }

    [StringLength(64)]
    public string? SkinTone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AIStylingHistory> AIStylingHistories { get; set; } = new List<AIStylingHistory>();

    [InverseProperty("User")]
    public virtual ICollection<AdsClickLog> AdsClickLogs { get; set; } = new List<AdsClickLog>();

    [InverseProperty("User")]
    public virtual ICollection<AffiliateLink> AffiliateLinks { get; set; } = new List<AffiliateLink>();

    [InverseProperty("User")]
    public virtual ICollection<AnalyticsLog> AnalyticsLogs { get; set; } = new List<AnalyticsLog>();

    [ForeignKey("AvatarImageId")]
    [InverseProperty("Users")]
    public virtual Image? AvatarImage { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Closet> Closets { get; set; } = new List<Closet>();

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Layout> Layouts { get; set; } = new List<Layout>();

    [InverseProperty("CreatedByNavigation")]
    public virtual ICollection<Mannequin> Mannequins { get; set; } = new List<Mannequin>();

    [InverseProperty("User")]
    public virtual ICollection<Outfit> Outfits { get; set; } = new List<Outfit>();

    [InverseProperty("User")]
    public virtual ICollection<ProductFilterLog> ProductFilterLogs { get; set; } = new List<ProductFilterLog>();

    [InverseProperty("HandledByNavigation")]
    public virtual ICollection<Report> ReportHandledByNavigations { get; set; } = new List<Report>();

    [InverseProperty("User")]
    public virtual ICollection<Report> ReportUsers { get; set; } = new List<Report>();

    [ForeignKey("RoleId")]
    [InverseProperty("Users")]
    public virtual Role? Role { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<SkinToneAnalysisLog> SkinToneAnalysisLogs { get; set; } = new List<SkinToneAnalysisLog>();

    [ForeignKey("SubscriptionPlanId")]
    [InverseProperty("Users")]
    public virtual SubscriptionPlan? SubscriptionPlan { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<TemporaryRole> TemporaryRoles { get; set; } = new List<TemporaryRole>();

    [ForeignKey("TenantId")]
    [InverseProperty("Users")]
    public virtual Tenant Tenant { get; set; } = null!;

    [InverseProperty("User")]
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

    [InverseProperty("User")]
    public virtual ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

    [InverseProperty("User")]
    public virtual ICollection<UserPromotion> UserPromotions { get; set; } = new List<UserPromotion>();

    [InverseProperty("User")]
    public virtual ICollection<VipPayment> VipPayments { get; set; } = new List<VipPayment>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

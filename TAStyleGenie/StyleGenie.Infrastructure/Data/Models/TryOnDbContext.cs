using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StyleGenie.Infrastructure.Data.Models;

public partial class TryOnDbContext : DbContext
{
    public TryOnDbContext(DbContextOptions<TryOnDbContext> options)
        : base(options)
    {
    }
    public DbSet<ApiKeys> ApiKeys { get; set; }
    public DbSet<Payment> Payments { get; set; }

    public virtual DbSet<AIStylingHistory> AIStylingHistories { get; set; }

    public virtual DbSet<AdsClickLog> AdsClickLogs { get; set; }

    public virtual DbSet<AffiliateLink> AffiliateLinks { get; set; }

    public virtual DbSet<AnalyticsLog> AnalyticsLogs { get; set; }

    public virtual DbSet<ApiCredential> ApiCredentials { get; set; }

    public virtual DbSet<AttributeType> AttributeTypes { get; set; }

    public virtual DbSet<AttributeValue> AttributeValues { get; set; }

    public virtual DbSet<Banner> Banners { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Closet> Closets { get; set; }

    public virtual DbSet<ClosetCategory> ClosetCategories { get; set; }

    public virtual DbSet<ClosetItem> ClosetItems { get; set; }

    public virtual DbSet<ClosetOutfitHistory> ClosetOutfitHistories { get; set; }

    public virtual DbSet<CreditWallet> CreditWallets { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<Item> Items { get; set; }

    public virtual DbSet<ItemAttribute> ItemAttributes { get; set; }

    public virtual DbSet<ItemCategory> ItemCategories { get; set; }

    public virtual DbSet<Layout> Layouts { get; set; }

    public virtual DbSet<Mannequin> Mannequins { get; set; }

    public virtual DbSet<Outfit> Outfits { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<ProductFilterLog> ProductFilterLogs { get; set; }

    public virtual DbSet<PromotionPlan> PromotionPlans { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<RolePermission> RolePermissions { get; set; }

    public virtual DbSet<SkinToneAnalysisLog> SkinToneAnalysisLogs { get; set; }

    public virtual DbSet<SkinToneColorRecommendation> SkinToneColorRecommendations { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<TemporaryRole> TemporaryRoles { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<TryOnJob> TryOnJobs { get; set; }

    public virtual DbSet<UsageLedger> UsageLedgers { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserPermission> UserPermissions { get; set; }

    public virtual DbSet<UserPromotion> UserPromotions { get; set; }

    public virtual DbSet<VipPayment> VipPayments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AIStylingHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AIStylin__3214EC07A4CDDEF3");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Outfit).WithMany(p => p.AIStylingHistories).HasConstraintName("FK__AIStyling__Outfi__681373AD");

            entity.HasOne(d => d.User).WithMany(p => p.AIStylingHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AIStyling__UserI__671F4F74");
        });

        modelBuilder.Entity<AdsClickLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdsClick__3214EC075D10E22E");

            entity.Property(e => e.ClickedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Banner).WithMany(p => p.AdsClickLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__AdsClickL__Banne__44CA3770");

            entity.HasOne(d => d.User).WithMany(p => p.AdsClickLogs).HasConstraintName("FK__AdsClickL__UserI__43D61337");
        });

        modelBuilder.Entity<AffiliateLink>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Affiliat__3214EC07E6E003D0");

            entity.HasOne(d => d.Item).WithMany(p => p.AffiliateLinks)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Affiliate__ItemI__3B40CD36");

            entity.HasOne(d => d.User).WithMany(p => p.AffiliateLinks).HasConstraintName("FK__Affiliate__UserI__3A4CA8FD");
        });

        modelBuilder.Entity<AnalyticsLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Analytic__3214EC077C51604A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.AnalyticsLogs).HasConstraintName("FK__Analytics__UserI__5224328E");
        });

        modelBuilder.Entity<ApiCredential>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiCrede__3214EC07E7083C79");

            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Tenant).WithMany(p => p.ApiCredentials).HasConstraintName("FK__ApiCreden__Tenan__628FA481");
        });

        modelBuilder.Entity<AttributeType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3214EC07AEA67981");
        });

        modelBuilder.Entity<AttributeValue>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attribut__3214EC07EFE08416");

            entity.Property(e => e.HexColor).IsFixedLength();

            entity.HasOne(d => d.AttributeType).WithMany(p => p.AttributeValues)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Attribute__Attri__08B54D69");
        });

        modelBuilder.Entity<Banner>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Banners__3214EC07F43F36C2");

            entity.Property(e => e.Active).HasDefaultValue(true);

            entity.HasOne(d => d.Image).WithMany(p => p.Banners).HasConstraintName("FK__Banners__ImageId__40058253");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07121B8EFA");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK__Categorie__Paren__75A278F5");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Categories).HasConstraintName("FK__Categorie__Tenan__74AE54BC");
        });

        modelBuilder.Entity<Closet>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Closets__3214EC0774301F9A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Closets)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Closets__UserId__25518C17");
        });

        modelBuilder.Entity<ClosetCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClosetCa__3214EC0758A2952E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Closet).WithMany(p => p.ClosetCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClosetCat__Close__2A164134");
        });

        modelBuilder.Entity<ClosetItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClosetIt__3214EC076E5D4416");

            entity.Property(e => e.AddedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Category).WithMany(p => p.ClosetItems).HasConstraintName("FK__ClosetIte__Categ__30C33EC3");

            entity.HasOne(d => d.Closet).WithMany(p => p.ClosetItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClosetIte__Close__2EDAF651");

            entity.HasOne(d => d.Item).WithMany(p => p.ClosetItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClosetIte__ItemI__2FCF1A8A");
        });

        modelBuilder.Entity<ClosetOutfitHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClosetOu__3214EC079EF6C131");

            entity.Property(e => e.SavedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Closet).WithMany(p => p.ClosetOutfitHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClosetOut__Close__3493CFA7");

            entity.HasOne(d => d.Outfit).WithMany(p => p.ClosetOutfitHistories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ClosetOut__Outfi__3587F3E0");
        });

        modelBuilder.Entity<CreditWallet>(entity =>
        {
            entity.HasKey(e => e.TenantId).HasName("PK__CreditWa__2E9B47E1CC818A41");

            entity.Property(e => e.TenantId).ValueGeneratedNever();
            entity.Property(e => e.RowVer)
                .IsRowVersion()
                .IsConcurrencyToken();
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Tenant).WithOne(p => p.CreditWallet)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CreditWal__Tenan__59063A47");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Feedback__3214EC07760EC4BD");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__UserId__489AC854");
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Images__3214EC076E7BD6E2");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Sha256Hex).IsFixedLength();

            entity.HasOne(d => d.Tenant).WithMany(p => p.Images).HasConstraintName("FK__Images__TenantId__66603565");
        });

        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Items__3214EC07E92F6770");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Items).HasConstraintName("FK__Items__CreatedBy__7D439ABD");

            entity.HasOne(d => d.PrimaryImage).WithMany(p => p.Items).HasConstraintName("FK__Items__PrimaryIm__7A672E12");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Items).HasConstraintName("FK__Items__TenantId__797309D9");
        });

        modelBuilder.Entity<ItemAttribute>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ItemAttr__3214EC0744632D9A");

            entity.HasOne(d => d.AttributeValue).WithMany(p => p.ItemAttributes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemAttri__Attri__0D7A0286");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemAttributes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemAttri__ItemI__0C85DE4D");
        });

        modelBuilder.Entity<ItemCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ItemCate__3214EC07EEAB2D81");

            entity.HasOne(d => d.Category).WithMany(p => p.ItemCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemCateg__Categ__03F0984C");

            entity.HasOne(d => d.Item).WithMany(p => p.ItemCategories)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ItemCateg__ItemI__02FC7413");
        });

        modelBuilder.Entity<Layout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Layouts__3214EC07B09C90F1");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Layouts).HasConstraintName("FK__Layouts__Created__17036CC0");
        });

        modelBuilder.Entity<Mannequin>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Mannequi__3214EC07570CBEAC");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Mannequins).HasConstraintName("FK__Mannequin__Creat__123EB7A3");
        });

        modelBuilder.Entity<Outfit>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Outfits__3214EC07EE48B688");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Layout).WithMany(p => p.Outfits).HasConstraintName("FK__Outfits__LayoutI__1BC821DD");

            entity.HasOne(d => d.Mannequin).WithMany(p => p.Outfits).HasConstraintName("FK__Outfits__Mannequ__1AD3FDA4");

            entity.HasOne(d => d.ResultImage).WithMany(p => p.Outfits).HasConstraintName("FK__Outfits__ResultI__1CBC4616");

            entity.HasOne(d => d.User).WithMany(p => p.Outfits)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Outfits__UserId__19DFD96B");

            entity.HasMany(d => d.Items).WithMany(p => p.Outfits)
                .UsingEntity<Dictionary<string, object>>(
                    "OutfitItem",
                    r => r.HasOne<Item>().WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OutfitIte__ItemI__22751F6C"),
                    l => l.HasOne<Outfit>().WithMany()
                        .HasForeignKey("OutfitId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__OutfitIte__Outfi__2180FB33"),
                    j =>
                    {
                        j.HasKey("OutfitId", "ItemId");
                        j.ToTable("OutfitItems");
                    });
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Permissi__3214EC07FBC55984");
        });

        modelBuilder.Entity<ProductFilterLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductF__3214EC0722ECFFD4");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.ProductFilterLogs).HasConstraintName("FK__ProductFi__UserI__6EC0713C");
        });

        modelBuilder.Entity<PromotionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Promotio__3214EC0726C1712B");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Reports__3214EC0783CF5574");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue("OPEN");

            entity.HasOne(d => d.HandledByNavigation).WithMany(p => p.ReportHandledByNavigations).HasConstraintName("FK__Reports__Handled__4E53A1AA");

            entity.HasOne(d => d.User).WithMany(p => p.ReportUsers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reports__UserId__4C6B5938");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Roles__3214EC07A0900E75");
        });

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__RolePerm__3214EC077ABA786C");

            entity.HasOne(d => d.Permission).WithMany(p => p.RolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__Permi__47DBAE45");

            entity.HasOne(d => d.Role).WithMany(p => p.RolePermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__RolePermi__RoleI__46E78A0C");
        });

        modelBuilder.Entity<SkinToneAnalysisLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SkinTone__3214EC07D321E484");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.SkinToneAnalysisLogs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__SkinToneA__UserI__756D6ECB");
        });

        modelBuilder.Entity<SkinToneColorRecommendation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SkinTone__3214EC07869C7FE8");

            entity.Property(e => e.HexCode).IsFixedLength();
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC0722E6570D");
        });

        modelBuilder.Entity<TemporaryRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Temporar__3214EC0789556771");

            entity.HasOne(d => d.Role).WithMany(p => p.TemporaryRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Temporary__RoleI__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.TemporaryRoles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Temporary__UserI__4F7CD00D");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tenants__3214EC07E4362A53");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Status).HasDefaultValue((byte)1);
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Transact__3214EC077BCA6749");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.User).WithMany(p => p.Transactions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Transacti__UserI__55F4C372");
        });

        modelBuilder.Entity<TryOnJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__TryOnJob__3214EC079CB4B82D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.ClothImage).WithMany(p => p.TryOnJobClothImages).HasConstraintName("FK__TryOnJobs__Cloth__6D0D32F4");

            entity.HasOne(d => d.LowerClothImage).WithMany(p => p.TryOnJobLowerClothImages).HasConstraintName("FK__TryOnJobs__Lower__6E01572D");

            entity.HasOne(d => d.ModelImage).WithMany(p => p.TryOnJobModelImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TryOnJobs__Model__6C190EBB");

            entity.HasOne(d => d.ResultImage).WithMany(p => p.TryOnJobResultImages).HasConstraintName("FK__TryOnJobs__Resul__6EF57B66");

            entity.HasOne(d => d.Tenant).WithMany(p => p.TryOnJobs)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TryOnJobs__Tenan__6A30C649");
        });

        modelBuilder.Entity<UsageLedger>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UsageLed__3214EC07D4736902");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Job).WithMany(p => p.UsageLedgers).HasConstraintName("FK_UsageLedger_TryOnJobs");

            entity.HasOne(d => d.Tenant).WithMany(p => p.UsageLedgers)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UsageLedg__Tenan__5DCAEF64");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC070E310D8A");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.TenantId).HasDefaultValue(1L);

            entity.HasOne(d => d.AvatarImage).WithMany(p => p.Users).HasConstraintName("FK_Users_AvatarImage");

            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("FK_Users_Roles");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.Users).HasConstraintName("FK_Users_SubscriptionPlans");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Users__TenantId__403A8C7D");
        });

        modelBuilder.Entity<UserPermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserPerm__3214EC07E14F76DE");

            entity.HasOne(d => d.Permission).WithMany(p => p.UserPermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPermi__Permi__4CA06362");

            entity.HasOne(d => d.User).WithMany(p => p.UserPermissions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPermi__UserI__4BAC3F29");
        });
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payments");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.OrderCode).IsRequired();
            entity.Property(p => p.Amount).IsRequired();
            entity.Property(p => p.Status)
                  .HasMaxLength(50)
                  .HasDefaultValue("PENDING");
            entity.Property(p => p.Description).HasColumnType("nvarchar(max)");
            entity.Property(p => p.PaymentLinkId).HasMaxLength(255);
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(p => p.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // 🔹 Quan hệ 1-n: User → Payments
            entity.HasOne(p => p.User)
                  .WithMany(u => u.Payments)
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<UserPromotion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserProm__3214EC07B2402347");

            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Promotion).WithMany(p => p.UserPromotions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__Promo__625A9A57");

            entity.HasOne(d => d.SubscriptionPlan).WithMany(p => p.UserPromotions).HasConstraintName("FK__UserPromo__Subsc__634EBE90");

            entity.HasOne(d => d.User).WithMany(p => p.UserPromotions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__UserPromo__UserI__6166761E");
        });

        modelBuilder.Entity<VipPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__VipPayme__3214EC0795C20CCC");

            entity.HasOne(d => d.Plan).WithMany(p => p.VipPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VipPaymen__PlanI__5AB9788F");

            entity.HasOne(d => d.Transaction).WithMany(p => p.VipPayments).HasConstraintName("FK__VipPaymen__Trans__5BAD9CC8");

            entity.HasOne(d => d.User).WithMany(p => p.VipPayments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__VipPaymen__UserI__59C55456");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

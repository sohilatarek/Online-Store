using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Categories;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace OnlineStore.Configurations
{
    /// <summary>
    /// EF Core configuration for Category entity
    /// Configures table mapping, constraints, and indexes
    /// </summary>
    internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
       
            builder.ConfigureByConvention();

       
            // ID is not auto-generated (manually assigned)
            builder.Property(x => x.Id)
                .ValueGeneratedNever();

            // ==========================================
            // COLUMN CONFIGURATIONS
            // ==========================================

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(OnlineStoreConsts.GeneralTextMaxLength); 

            builder.Property(x => x.NameEn)
                .IsRequired()
                .HasMaxLength(OnlineStoreConsts.GeneralTextMaxLength); 

            builder.Property(x => x.DescriptionAr)
                .IsRequired()
                .HasMaxLength(OnlineStoreConsts.DescriptionTextMaxLength); 

            builder.Property(x => x.DescriptionEn)
                .IsRequired()
                .HasMaxLength(OnlineStoreConsts.DescriptionTextMaxLength); 

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(x => x.DisplayOrder)
                .IsRequired()
                .HasDefaultValue(0);

            // ==========================================
            // INDEXES FOR PERFORMANCE
            // ==========================================

            // Index for filtering by active status
            // Query: WHERE IsActive = 1
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_Categories_IsActive");

            // Index for sorting by display order
            // Query: ORDER BY DisplayOrder
            builder.HasIndex(x => x.DisplayOrder)
                .HasDatabaseName("IX_Categories_DisplayOrder");

            // Composite index for tenant + active filtering
            // Query: WHERE TenantId = X AND IsActive = 1
            builder.HasIndex(x => new { x.TenantId, x.IsActive })
                .HasDatabaseName("IX_Categories_TenantId_IsActive");

            // ==========================================
            // UNIQUE CONSTRAINTS
            // ==========================================

            // Unique constraint for English name per tenant (excluding deleted)
            builder.HasIndex(x => new { x.NameEn, x.TenantId })
                .IsUnique()
                .HasDatabaseName("UQ_Categories_NameEn_TenantId")
                .HasFilter("[IsDeleted] = 0");

            // Unique constraint for Arabic name per tenant (excluding deleted)
            builder.HasIndex(x => new { x.NameAr, x.TenantId })
                .IsUnique()
                .HasDatabaseName("UQ_Categories_NameAr_TenantId")
                .HasFilter("[IsDeleted] = 0");


            builder.ToTable("Categories");
        }
    }
}
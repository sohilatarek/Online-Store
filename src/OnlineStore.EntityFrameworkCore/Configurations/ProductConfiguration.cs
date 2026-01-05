using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineStore.Products;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace OnlineStore.Configurations
{
  
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ConfigureByConvention();

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

            builder.Property(x => x.SKU)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Stock Keeping Unit - unique product identifier");

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasComment("Product price in system currency");

            builder.Property(x => x.StockQuantity)
                .IsRequired()
                .HasDefaultValue(0)
                .HasComment("Available inventory quantity");

            builder.Property(x => x.IsActive)
                .IsRequired()
                .HasDefaultValue(true)
                .HasComment("Whether product is active and can be managed");

            builder.Property(x => x.IsPublished)
                .IsRequired()
                .HasDefaultValue(false)
                .HasComment("Whether product is visible to customers");

            // ==========================================
            // FOREIGN KEY RELATIONSHIPS
            // ==========================================

            // Product belongs to Category (Many-to-One)
            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // ==========================================
            // CHECK CONSTRAINTS (Business Rules)
            // ==========================================

            // Price must be >= 0
            builder.HasCheckConstraint(
                "CK_Products_Price",
                "[Price] >= 0");

            // Stock quantity must be >= 0
            builder.HasCheckConstraint(
                "CK_Products_StockQuantity",
                "[StockQuantity] >= 0");

            // ==========================================
            // INDEXES FOR PERFORMANCE
            // ==========================================

            // Index for SKU lookups
            builder.HasIndex(x => x.SKU)
                .HasDatabaseName("IX_Products_SKU");

            // Unique constraint for SKU per tenant (excluding deleted)
            builder.HasIndex(x => new { x.SKU, x.TenantId })
                .IsUnique()
                .HasDatabaseName("UQ_Products_SKU_TenantId")
                .HasFilter("[IsDeleted] = 0");

            // Index for filtering active products
            // Query: WHERE IsActive = 1
            builder.HasIndex(x => x.IsActive)
                .HasDatabaseName("IX_Products_IsActive");

            // Index for filtering published products
            // Query: WHERE IsPublished = 1
            builder.HasIndex(x => x.IsPublished)
                .HasDatabaseName("IX_Products_IsPublished");

            // Composite index for tenant + active + published
            // Query: WHERE TenantId = X AND IsActive = 1 AND IsPublished = 1
            builder.HasIndex(x => new { x.TenantId, x.IsActive, x.IsPublished })
                .HasDatabaseName("IX_Products_TenantId_IsActive_IsPublished");

            // Composite index for category filtering
            // Query: WHERE CategoryId = X AND IsActive = 1
            builder.HasIndex(x => new { x.CategoryId, x.IsActive })
                .HasDatabaseName("IX_Products_CategoryId_IsActive");

            // Index for price range filtering
            // Query: WHERE Price BETWEEN X AND Y
            builder.HasIndex(x => x.Price)
                .HasDatabaseName("IX_Products_Price");

            // Index for stock filtering
            // Query: WHERE StockQuantity <= 10
            builder.HasIndex(x => x.StockQuantity)
                .HasDatabaseName("IX_Products_StockQuantity");

            // Covering index for product listing with frequently accessed columns
            // Improves SELECT performance by including data in index
            builder.HasIndex(x => x.CategoryId)
                .HasDatabaseName("IX_Products_CategoryId_Covering")
                .IncludeProperties(p => new
                {
                    p.NameEn,
                    p.NameAr,
                    p.Price,
                    p.StockQuantity,
                    p.IsActive,
                    p.IsPublished
                });

            builder.ToTable("Products");
        }
    }
}
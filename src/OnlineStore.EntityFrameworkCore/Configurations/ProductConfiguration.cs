using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Volo.Abp.EntityFrameworkCore.Modeling;
using OnlineStore;
using OnlineStore.Products;


namespace OnlineStore.Configurations
{
    internal class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ConfigureByConvention();

            builder.Property(x => x.NameAr).HasMaxLength(OnlineStoreConsts.GeneralTextMaxLength).IsRequired();
            builder.Property(x => x.NameEn).HasMaxLength(OnlineStoreConsts.GeneralTextMaxLength).IsRequired();
            builder.Property(x => x.DescriptionAr).HasMaxLength(OnlineStoreConsts.DescriptionTextMaxLength).IsRequired();
            builder.Property(x => x.DescriptionEn).HasMaxLength(OnlineStoreConsts.DescriptionTextMaxLength).IsRequired();

            builder.HasOne(x => x.Category)
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .IsRequired();

            builder.ToTable("Products");
        }
    }
}
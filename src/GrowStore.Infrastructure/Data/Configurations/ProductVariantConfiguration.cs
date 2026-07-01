using GrowStore.Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GrowStore.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.ProductId)
            .IsRequired();

        builder.Property(v => v.Color)
            .HasMaxLength(50);

        builder.Property(v => v.Size)
            .HasMaxLength(50);

        builder.Property(v => v.Stock)
            .IsRequired();

        builder.Property(v => v.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(v => v.Sku)
            .HasMaxLength(100);

        builder.HasIndex(v => v.Sku)
            .IsUnique();

        builder.Property(v => v.CreatedAt)
            .IsRequired();

        builder.Property(v => v.UpdatedAt)
            .IsRequired();

        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

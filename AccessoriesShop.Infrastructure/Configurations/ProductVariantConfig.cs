using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class ProductVariantConfig : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Sku)
                .HasMaxLength(100);

            builder.Property(x => x.Name)
                .HasMaxLength(200);

            builder.Property(x => x.StockQuantity)
                .HasDefaultValue(0);

            builder.Property(x => x.Price)
                .HasPrecision(18, 2);

            builder.Property(x => x.ImageUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Color)
                .HasMaxLength(50);

            builder.Property(x => x.Size)
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(v => v.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(v => v.OrderItems)
                .WithOne(oi => oi.Variant)
                .HasForeignKey(oi => oi.VariantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(v => v.Sku).IsUnique();
        }
    }
}

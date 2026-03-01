using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class ProductAttributeConfig : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Value)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(pa => pa.Product)
                .WithMany(p => p.productAttributes)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pa => pa.Attribute)
                .WithMany(a => a.ProductAttributes)
                .HasForeignKey(pa => pa.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

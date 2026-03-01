using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class ProductCompatibilityConfig : IEntityTypeConfiguration<ProductCompatibility>
    {
        public void Configure(EntityTypeBuilder<ProductCompatibility> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Note)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(pc => pc.Product)
                .WithMany(p => p.productCompatibilities)
                .HasForeignKey(pc => pc.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pc => pc.Device)
                .WithMany(d => d.ProductCompatibilities)
                .HasForeignKey(pc => pc.DeviceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

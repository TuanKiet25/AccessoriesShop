using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class CustomOrderConfig : IEntityTypeConfiguration<CustomOrder>
    {
        public void Configure(EntityTypeBuilder<CustomOrder> builder)
        {
            builder.ToTable("CustomOrders");

            builder.Property(c => c.Color).HasMaxLength(150);
            builder.Property(c => c.Material).HasMaxLength(150);
            builder.Property(c => c.TextContent).HasMaxLength(2000);
            builder.Property(c => c.Note).HasMaxLength(2000);
            builder.Property(c => c.Status).IsRequired().HasMaxLength(50);
            builder.Property(c => c.CustomerName).HasMaxLength(200);
            builder.Property(c => c.CustomerEmail).HasMaxLength(200);
            builder.Property(c => c.CustomerPhone).HasMaxLength(50);

            builder.HasOne(c => c.Account)
                   .WithMany()
                   .HasForeignKey(c => c.AccountId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.ProductBase)
                   .WithMany()
                   .HasForeignKey(c => c.ProductBaseId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.Files)
                   .WithOne(f => f.CustomOrder)
                   .HasForeignKey(f => f.CustomOrderId);
        }
    }
}

using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class PaymentConfig : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasPrecision(18, 2);

            builder.Property(x => x.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("USD");

            builder.Property(x => x.PaymentMethod)
                .HasMaxLength(50);

            builder.Property(x => x.TransactionCode)
                .HasMaxLength(100);

            builder.Property(x => x.Status)
                .HasMaxLength(50);

            builder.Property(x => x.BankCode)
                .HasMaxLength(50);

            builder.Property(x => x.ResponseCode)
                .HasMaxLength(50);

            builder.Property(x => x.ResponseMessage)
                .HasMaxLength(500);

            builder.Property(x => x.PaymentUrl)
                .HasMaxLength(500);

            builder.Property(x => x.TransactionRef)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(p => p.Order)
                .WithMany(o => o.Payments)
                .HasForeignKey(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(p => p.TransactionRef).IsUnique();
        }
    }
}

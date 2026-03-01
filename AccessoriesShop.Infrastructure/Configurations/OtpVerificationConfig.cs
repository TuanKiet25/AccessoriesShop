using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class OtpVerificationConfig : IEntityTypeConfiguration<OtpVerification>
    {
        public void Configure(EntityTypeBuilder<OtpVerification> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AccountId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.OtpCode)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(x => x.IsUsed)
                .HasDefaultValue(false);

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            // Relationships
            builder.HasOne(ov => ov.Account)
                .WithMany(a => a.OtpVerifications)
                .HasForeignKey(ov => ov.AccountId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ov => new { ov.AccountId, ov.OtpCode }).IsUnique();
        }
    }
}

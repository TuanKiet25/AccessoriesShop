using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Configurations
{
	public class PromotionConfig : IEntityTypeConfiguration<Promotion>
	{
		public void Configure(EntityTypeBuilder<Promotion> builder)
		{
			builder.ToTable("Promotions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Name)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(x => x.Description)
				.HasMaxLength(1000);

			builder.Property(x => x.DiscountType)
				.IsRequired()
				.HasMaxLength(50);

			builder.Property(x => x.DiscountValue)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(x => x.MaxDiscountAmount)
				.HasColumnType("decimal(18,2)");

			builder.Property(x => x.MinOrderValue)
				.HasColumnType("decimal(18,2)");

			builder.Property(x => x.IsActive)
				.HasDefaultValue(true);

			builder.HasOne(x => x.Product)
				.WithMany(x => x.Promotions)
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasCheckConstraint("CK_Promotions_DiscountValue", "[DiscountValue] >= 0");
			builder.HasCheckConstraint("CK_Promotions_DateRange", "[EndDate] >= [StartDate]");
		}
	}
}

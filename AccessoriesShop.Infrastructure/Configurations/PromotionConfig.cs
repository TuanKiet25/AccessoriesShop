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
			builder.ToTable("Promotions", t =>
			{
				t.HasCheckConstraint("CK_Promotions_DiscountValue", "\"DiscountValue\" > 0");
				t.HasCheckConstraint("CK_Promotions_DateRange", "\"EndDate\" >= \"StartDate\"");
				t.HasCheckConstraint(
					"CK_Promotions_PercentageRange",
					"\"IsPercentage\" = FALSE OR (\"DiscountValue\" >= 0 AND \"DiscountValue\" <= 100)"
				);
			});

			builder.HasKey(x => x.Id);

			builder.Property(x => x.ProductId)
				.IsRequired();

			builder.Property(x => x.Name)
				.IsRequired()
				.HasMaxLength(200);

			builder.Property(x => x.DiscountValue)
				.HasColumnType("decimal(18,2)")
				.IsRequired();

			builder.Property(x => x.IsPercentage)
				.IsRequired();

			builder.Property(x => x.StartDate)
				.IsRequired();

			builder.Property(x => x.EndDate)
				.IsRequired();

			builder.Property(x => x.IsActive)
				.HasDefaultValue(true);

			builder.HasOne(x => x.Product)
				.WithMany(x => x.Promotions)
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}

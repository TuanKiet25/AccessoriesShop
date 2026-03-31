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
	public class RatingConfig : IEntityTypeConfiguration<Rating>
	{
		public void Configure(EntityTypeBuilder<Rating> builder)
		{
			builder.ToTable("Ratings");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.Star)
				.IsRequired();

			builder.Property(x => x.Comment)
				.HasMaxLength(1000);

			builder.Property(x => x.IsVisible)
				.HasDefaultValue(true);

			builder.Property(x => x.CreatedAt)
				.HasDefaultValueSql("GETUTCDATE()");

			builder.HasOne(x => x.Product)
				.WithMany(x => x.Ratings)
				.HasForeignKey(x => x.ProductId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.Account)
				.WithMany(x => x.Ratings)
				.HasForeignKey(x => x.AccountId)
				.OnDelete(DeleteBehavior.Restrict);

			builder.HasIndex(x => new { x.ProductId, x.AccountId })
				.IsUnique();

			builder.HasCheckConstraint("CK_Ratings_Star", "[Star] >= 1 AND [Star] <= 5");
		}
	}
}

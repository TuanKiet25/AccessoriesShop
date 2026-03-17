using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class CustomOrderFileConfig : IEntityTypeConfiguration<CustomOrderFile>
    {
        public void Configure(EntityTypeBuilder<CustomOrderFile> builder)
        {
            builder.ToTable("CustomOrderFiles");

            builder.Property(f => f.FileUrl)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(f => f.FileName)
                   .HasMaxLength(255);
        }
    }
}

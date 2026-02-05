using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Configurations
{
    public class AttributeConfig : IEntityTypeConfiguration<Attributes>
    {
        public void Configure(EntityTypeBuilder<Attributes> builder)
        {
            builder.HasMany(p => p.ProductAttributes)
                .WithOne(a => a.Attribute)
                .HasForeignKey(p => p.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

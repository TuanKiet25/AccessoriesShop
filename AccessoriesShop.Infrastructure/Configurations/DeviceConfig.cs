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
    public class DeviceConfig : IEntityTypeConfiguration<Device>
    {
        public void Configure(EntityTypeBuilder<Device> builder)
        {
            builder.HasMany(p => p.ProductCompatibilities)
                .WithOne(d => d.Device)
                .HasForeignKey(p => p.DeviceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

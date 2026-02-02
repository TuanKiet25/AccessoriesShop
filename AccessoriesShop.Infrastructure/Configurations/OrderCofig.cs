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
    public class OrderCofig : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasOne(o => o.Account)
                   .WithMany(a => a.Orders)
                   .HasForeignKey(o => o.AccountId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

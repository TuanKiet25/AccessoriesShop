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
    public class CartConfig : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
           builder.HasMany(builder => builder.CartItems)
                .WithOne(cartItem => cartItem.Cart)
                .HasForeignKey(cartItem => cartItem.CartId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(cart => cart.Account)
                .WithOne(account => account.Cart)
                .HasForeignKey<Cart>(cart => cart.AccountId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

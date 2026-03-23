using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public Cart? Cart { get; set; }
        public Guid ProductVariantId { get; set; }
        public ProductVariant? ProductVariant { get; set; }
        public int Quantity { get; set; }
    }  
}

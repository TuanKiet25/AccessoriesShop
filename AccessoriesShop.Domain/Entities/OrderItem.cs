using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Order? Order { get; set; }
        public Guid VariantId { get; set; }
        public ProductVariant? Variant { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }


    }
}

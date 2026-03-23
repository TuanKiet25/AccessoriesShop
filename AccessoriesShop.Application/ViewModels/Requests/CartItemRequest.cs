using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CartItemRequest
    {
        public Guid CartId { get; set; }
        public Guid ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}

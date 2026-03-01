using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateOrderItemRequest
    {
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
    }
}

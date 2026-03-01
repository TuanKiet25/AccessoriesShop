using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class OrderItemResponse
    {
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}

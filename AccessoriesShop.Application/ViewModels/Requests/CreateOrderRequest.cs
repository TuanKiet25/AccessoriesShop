using AccessoriesShop.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreateOrderRequest
    {
        public Guid AccountId { get; set; }
        public List<CreateOrderItemRequest>? OrderItems { get; set; }
    }
}

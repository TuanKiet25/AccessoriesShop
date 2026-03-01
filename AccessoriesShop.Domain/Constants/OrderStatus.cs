using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Constants
{
    public class OrderStatus
    {
        public const string Pending = "Pending"; 
        public const string Confirmed = "Confirmed"; 
        public const string Shipping = "Shipping";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
        public const string Returned = "Returned";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Constants
{
    public class CustomOrderStatus
    {
        public const string Requested = "Requested";
        public const string Quoted = "Quoted";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string Paid = "Paid";
        public const string InProduction = "InProduction";
        public const string Completed = "Completed";
        public const string Cancelled = "Cancelled";
    }
}

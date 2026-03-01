using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Constants
{
    public class PaymentStatus
    {
        public const string Success = "Success";
        public const string Failed = "Failed";
        public const string Pending = "Pending";
        public const string Canceled = "Canceled";
    }
    public class PaymentGateway
    {
        public const string Vnpay = "Vnpay";
        public const string Paypal = "Paypal";
        public const string PayOS = "PayOS";
        public const string Momo = "Momo";
        public const string COD = "COD";
    }
}

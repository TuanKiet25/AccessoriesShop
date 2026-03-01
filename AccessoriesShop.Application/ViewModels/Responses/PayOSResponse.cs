using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class PayOSResponse
    {
        public bool Success { get; set; }
        public string? PaymentUrl { get; set; }
        public string? QrCode { get; set; }
        public string? Message { get; set; }
        public string? TransactionId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ProcessPaymentResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? OrderId { get; set; }
        public string? TransactionCode { get; set; }
        public decimal? Amount { get; set; }
        public string? BankCode { get; set; }
        public string? CardType { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Requests
{
    public class CreatePaymentRequest
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; } = "VND";
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
        public string? Status { get; set; }
        public string? BankCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public string? PaymentUrl { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
        public string? TransactionRef { get; set; }
    }
}

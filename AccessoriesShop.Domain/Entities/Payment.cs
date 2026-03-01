using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Payment : BaseEntity
    {
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
        public string? PaymentMethod { get; set; }
        public string? TransactionCode { get; set; }
        public string? Status { get; set; }
        public string? BankCode { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public string? PaymentUrl { get; set; }
        public string? ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
        public string TransactionRef { get; set; } = null!;
        public virtual Order Order { get; set; } = null!;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Order : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public List<OrderItem>? OrderItems { get; set; }
        public List<Payment>? Payments { get; set; }

        /// <summary>
        /// Automatically calculates the TotalAmount by summing up all OrderItem prices
        /// </summary>
        public void CalculateTotalAmount()
        {
            if (OrderItems != null && OrderItems.Count > 0)
            {
                TotalAmount = OrderItems.Sum(oi => oi.Price * oi.Quantity);
            }
            else
            {
                TotalAmount = 0;
            }
        }
    }
}


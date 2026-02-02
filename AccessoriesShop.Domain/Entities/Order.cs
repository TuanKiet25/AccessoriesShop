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
    }
}

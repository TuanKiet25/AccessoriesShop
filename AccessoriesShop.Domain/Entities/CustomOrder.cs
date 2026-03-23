using AccessoriesShop.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class CustomOrder : BaseEntity
    {
        public Guid? AccountId { get; set; }
        public Account? Account { get; set; }

        public Guid? ProductBaseId { get; set; }
        public Product? ProductBase { get; set; }

        public string? Color { get; set; }
        public string? Material { get; set; }
        public string? TextContent { get; set; }
        public string? Note { get; set; }
        public int Quantity { get; set; }

        public decimal? EstimatedPrice { get; set; }
        public decimal? FinalPrice { get; set; }
        public string Status { get; set; } = CustomOrderStatus.Requested;
        public DateTime? EstimatedDeliveryDate { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public string? CustomerPhone { get; set; }

        public List<CustomOrderFile>? Files { get; set; }
    }
}

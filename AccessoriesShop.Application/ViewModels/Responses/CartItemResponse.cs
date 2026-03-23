using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class CartItemResponse
    {
        public Guid Id { get; set; }
        public Guid CartId { get; set; }
        public Guid ProductVariantId { get; set; }
        public string? ProductVariantName { get; set; }
        public string? ProductVariantImageUrl { get; set; }
        public string? ProductVariantColor { get; set; }
        public string? ProductVariantSize { get; set; }
        public decimal ProductVariantPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => ProductVariantPrice * Quantity;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Product : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid BrandId { get; set; }
        public Brand? Brand { get; set; }   
        public Guid CategoryId { get; set; }
        public Category? Category { get; set; }
        public List<ProductVariant>? Variants { get; set; }
        public List<ProductAttribute>? productAttributes { get; set; }
        public List<ProductCompatibility>? productCompatibilities { get; set; }
    }
}

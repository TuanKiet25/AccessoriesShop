using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }
        public Guid AttributeId { get; set; }
        public Attributes? Attribute { get; set; }
        public string? Value { get; set; }

    }
}

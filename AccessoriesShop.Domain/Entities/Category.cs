using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Category : BaseEntity
    {
        public Guid? ParentId { get; set; }
        public string? Name { get; set; }
        public string? Slug { get; set; }
        public List<Product>? Products { get; set; }
        public Category? Parent { get; set; }
        public List<Category>? Children { get; set; }
    }
}

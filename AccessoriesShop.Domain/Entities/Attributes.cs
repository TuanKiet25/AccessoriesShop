using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Attributes : BaseEntity
    {
        public string? Name { get; set; }
        public string? DataType { get; set; }
        public List<ProductAttribute>? ProductAttributes { get; set; }
    }
}

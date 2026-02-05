using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Device : BaseEntity
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<ProductCompatibility>? ProductCompatibilities { get; set; }
    }
}

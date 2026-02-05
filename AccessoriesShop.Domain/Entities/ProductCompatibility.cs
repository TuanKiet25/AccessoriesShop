using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class ProductCompatibility : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Product? Product { get; set; }   
        public Guid DeviceId { get; set; }
        public Device? Device { get; set; }
        public string? Note { get; set; }   
    }
}

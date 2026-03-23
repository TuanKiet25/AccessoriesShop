using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class CustomOrderFile : BaseEntity
    {
        public Guid CustomOrderId { get; set; }
        public CustomOrder? CustomOrder { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string? FileName { get; set; }
    }
}

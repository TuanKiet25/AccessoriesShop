using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Cart : BaseEntity
    {
        public Guid AccountId { get; set; }
        public Account? Account { get; set; }
        public List<CartItem>? CartItems { get; set; }
    }
}

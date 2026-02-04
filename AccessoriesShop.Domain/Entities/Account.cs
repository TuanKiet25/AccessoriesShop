using AccessoriesShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Account : BaseEntity
    {
        public string Username { get; set; } 
        public string PasswordHash { get; set; } 
        public string Email { get; set; } 
        public string PhoneNumber { get; set; }
        public Role Role { get; set; } 
        public List<Order>? Orders { get; set; } 
    }
}

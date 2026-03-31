using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Domain.Entities
{
    public class Address : BaseEntity
    {
        public Guid AccountId { get; set; }
        required
        public string ProvinceCode { get; set; }
        required
        public string DistrictCode { get; set; }
        required
        public string WardCode { get; set; }
        required
        public string StreetAddress { get; set; }
        public bool IsDefault { get; set; }

        public virtual Account? Account { get; set; }
    }
}

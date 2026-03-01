using AccessoriesShop.Domain.Constants;
using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IRepositories
{
    public interface IPaymentRepository : IGenericRepository<Payment>
    {
        Task<Payment> GetByOrderCodeAsync(string code, string gateWay);
    }
}

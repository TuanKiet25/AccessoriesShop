using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class AccountRepository : GenericRepository<Account>, IAccountRepository
    {
        public AccountRepository(AppDbContext context) : base(context)
        {
        }
    }
}

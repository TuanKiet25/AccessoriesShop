using AccessoriesShop.Application.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application
{
    public interface IUnitOfWork
    {
        IAccountRepository Accounts { get; }
        Task<int> SaveChangesAsync();
    }
}

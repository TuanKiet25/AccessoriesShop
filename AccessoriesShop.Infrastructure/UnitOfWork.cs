using AccessoriesShop.Application;
using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        public readonly AppDbContext _context;
        public IAccountRepository Accounts { get; }
        public UnitOfWork(AppDbContext context, IAccountRepository _account)
        {
            _context = context;
            Accounts = _account;

        }

 

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

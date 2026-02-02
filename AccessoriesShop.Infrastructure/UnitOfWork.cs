using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure
{
    public class UnitOfWork
    {
        public readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
           
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}

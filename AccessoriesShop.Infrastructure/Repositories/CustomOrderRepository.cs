using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class CustomOrderRepository : GenericRepository<CustomOrder>, ICustomOrderRepository
    {
        public CustomOrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}

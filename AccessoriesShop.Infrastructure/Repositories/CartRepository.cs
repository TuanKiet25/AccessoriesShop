using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context)
        {
        }
    }
}

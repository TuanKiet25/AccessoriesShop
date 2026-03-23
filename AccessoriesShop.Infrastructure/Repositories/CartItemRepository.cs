using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }
    }
}

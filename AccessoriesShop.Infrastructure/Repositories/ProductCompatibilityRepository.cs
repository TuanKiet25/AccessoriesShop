using AccessoriesShop.Application.Interfaces.Repositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class ProductCompatibilityRepository : GenericRepository<ProductCompatibility>, IProductCompatibilityRepository
    {
        public ProductCompatibilityRepository(AppDbContext context) : base(context)
        {
        }
    }
}

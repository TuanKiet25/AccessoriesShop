using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class ProductVariantRepository : GenericRepository<ProductVariant>, IProductVariantRepository
    {
        public ProductVariantRepository(AppDbContext context) : base(context)
        {
        }
    }
}

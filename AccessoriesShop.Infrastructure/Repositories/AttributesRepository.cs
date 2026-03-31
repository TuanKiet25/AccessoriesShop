using AccessoriesShop.Application.Interfaces.Repositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class AttributesRepository : GenericRepository<Attributes>, IAttributesRepository
    {
        public AttributesRepository(AppDbContext context) : base(context)
        {
        }
    }
}

using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class AddressRepository : GenericRepository<Address>, IAddressRepository
    {
        public AddressRepository(AppDbContext context) : base(context)
        {
        }
    }
}

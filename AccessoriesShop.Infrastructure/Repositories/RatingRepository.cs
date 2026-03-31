using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Domain.Entities;


namespace AccessoriesShop.Infrastructure.Repositories
{
	public class RatingRepository : GenericRepository<Rating>, IRatingRepository
	{
		public RatingRepository(AppDbContext context) : base(context)
		{
		}
	}
}

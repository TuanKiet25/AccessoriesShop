using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IRepositories
{
	public interface IRatingRepository : IGenericRepository<Rating>
	{
		Task<Rating?> GetByProductAndAccountAsync(Guid productId, Guid accountId);
		Task<double> GetAverageRatingAsync(Guid productId);
		Task<int> GetTotalRatingsAsync(Guid productId);
	}
}

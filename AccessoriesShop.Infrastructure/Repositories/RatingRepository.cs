using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Repositories
{
	public class RatingRepository : GenericRepository<Rating>, IRatingRepository
	{
		private readonly AppDbContext _context;

		public RatingRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<Rating?> GetByProductAndAccountAsync(Guid productId, Guid accountId)
		{
			return await _context.Ratings
				.FirstOrDefaultAsync(x => x.ProductId == productId && x.AccountId == accountId);
		}

		public async Task<double> GetAverageRatingAsync(Guid productId)
		{
			var avg = await _context.Ratings
				.Where(x => x.ProductId == productId && x.IsVisible)
				.Select(x => (double?)x.Star)
				.AverageAsync();

			return avg ?? 0;
		}

		public async Task<int> GetTotalRatingsAsync(Guid productId)
		{
			return await _context.Ratings.CountAsync(x => x.ProductId == productId && x.IsVisible);
		}
	}
}

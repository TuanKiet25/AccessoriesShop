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
	public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
	{
		private readonly AppDbContext _context;

		public PromotionRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<Promotion?> GetActivePromotionByProductIdAsync(Guid productId, DateTime utcNow)
		{
			return await _context.Promotions
				.Where(x => x.ProductId == productId
							&& x.IsActive
							&& x.StartDate <= utcNow
							&& x.EndDate >= utcNow)
				.OrderByDescending(x => x.DiscountValue)
				.FirstOrDefaultAsync();
		}
	}
}

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
		public RatingRepository(AppDbContext context) : base(context)
		{
		}
	}
}

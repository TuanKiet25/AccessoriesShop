using AccessoriesShop.Application.ViewModels.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
	public interface IRatingService
	{
		Task CreateOrUpdateAsync(Guid accountId, CreateRatingRequest request);
		Task<double> GetAverageAsync(Guid productId);
		Task<int> GetTotalAsync(Guid productId);
	}
}

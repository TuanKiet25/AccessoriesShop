using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
	public interface IPromotionService
	{
		Task CreateAsync(CreatePromotionRequest request);
		Task<Promotion?> GetActiveByProductIdAsync(Guid productId);
		decimal CalculateDiscountedPrice(decimal originalPrice, Promotion? promotion);
	}
}

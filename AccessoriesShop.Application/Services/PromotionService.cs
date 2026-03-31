using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
	public class PromotionService : IPromotionService
	{
		private readonly IPromotionRepository _promotionRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PromotionService(IPromotionRepository promotionRepository, IUnitOfWork unitOfWork)
		{
			_promotionRepository = promotionRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task CreateAsync(CreatePromotionRequest request)
		{
			var entity = new Promotion
			{
				ProductId = request.ProductId,
				Name = request.Name,
				Description = request.Description,
				DiscountType = request.DiscountType,
				DiscountValue = request.DiscountValue,
				MaxDiscountAmount = request.MaxDiscountAmount,
				MinOrderValue = request.MinOrderValue,
				StartDate = request.StartDate,
				EndDate = request.EndDate,
				IsActive = true
			};

			await _promotionRepository.AddAsync(entity);
			await _unitOfWork.SaveChangesAsync();
		}

		public Task<Promotion?> GetActiveByProductIdAsync(Guid productId)
			=> _promotionRepository.GetActivePromotionByProductIdAsync(productId, DateTime.UtcNow);

		public decimal CalculateDiscountedPrice(decimal originalPrice, Promotion? promotion)
		{
			if (promotion is null) return originalPrice;

			decimal discount = 0;
			if (promotion.DiscountType.Equals("Percent", StringComparison.OrdinalIgnoreCase))
			{
				discount = originalPrice * promotion.DiscountValue / 100m;
				if (promotion.MaxDiscountAmount.HasValue && discount > promotion.MaxDiscountAmount.Value)
				{
					discount = promotion.MaxDiscountAmount.Value;
				}
			}
			else if (promotion.DiscountType.Equals("FixedAmount", StringComparison.OrdinalIgnoreCase))
			{
				discount = promotion.DiscountValue;
			}

			var finalPrice = originalPrice - discount;
			return finalPrice < 0 ? 0 : finalPrice;
		}
	}
}

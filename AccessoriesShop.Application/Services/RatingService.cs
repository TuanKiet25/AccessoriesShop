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
	public class RatingService : IRatingService
	{
		private readonly IRatingRepository _ratingRepository;
		private readonly IUnitOfWork _unitOfWork;

		public RatingService(IRatingRepository ratingRepository, IUnitOfWork unitOfWork)
		{
			_ratingRepository = ratingRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task CreateOrUpdateAsync(Guid accountId, CreateRatingRequest request)
		{
			var existed = await _ratingRepository.GetByProductAndAccountAsync(request.ProductId, accountId);
			if (existed is null)
			{
				var rating = new Rating
				{
					ProductId = request.ProductId,
					AccountId = accountId,
					Star = request.Star,
					Comment = request.Comment,
					CreatedAt = DateTime.UtcNow,
					IsVisible = true
				};

				await _ratingRepository.AddAsync(rating);
			}
			else
			{
				existed.Star = request.Star;
				existed.Comment = request.Comment;
				existed.UpdatedAt = DateTime.UtcNow;
				_ratingRepository.UpdateAsync(existed);
			}

			await _unitOfWork.SaveChangesAsync();
		}

		public Task<double> GetAverageAsync(Guid productId)
			=> _ratingRepository.GetAverageRatingAsync(productId);

		public Task<int> GetTotalAsync(Guid productId)
			=> _ratingRepository.GetTotalRatingsAsync(productId);
	}
}

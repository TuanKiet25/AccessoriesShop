using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
	public interface IRatingService
	{
		Task<ServiceResult<IEnumerable<RatingResponse>>> GetAllAsync();
		Task<ServiceResult<RatingResponse>> GetByIdAsync(Guid id);
		Task<ServiceResult<RatingResponse>> CreateAsync(CreateRatingRequest request);
		Task<ServiceResult<RatingResponse>> UpdateAsync(Guid id, UpdateRatingRequest request);
		Task<ServiceResult<bool>> DeleteAsync(Guid id);
	}
}

using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;


namespace AccessoriesShop.Application.Services
{
	public class RatingService : IRatingService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public RatingService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ServiceResult<IEnumerable<RatingResponse>>> GetAllAsync()
		{
			try
			{
				var entities = await _unitOfWork.Ratings.GetAllAsync(x => true);
				return new ServiceResult<IEnumerable<RatingResponse>>
				{
					IsSuccess = true,
					Data = _mapper.Map<IEnumerable<RatingResponse>>(entities)
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<IEnumerable<RatingResponse>>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<RatingResponse>> GetByIdAsync(Guid id)
		{
			try
			{
				var entity = await _unitOfWork.Ratings.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<RatingResponse>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Rating not found."
					};
				}

				return new ServiceResult<RatingResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<RatingResponse>(entity)
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<RatingResponse>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<RatingResponse>> CreateAsync(CreateRatingRequest request)
		{
			try
			{
				var entity = _mapper.Map<Rating>(request);
				entity.Id = Guid.NewGuid();
				entity.CreatedAt = DateTime.UtcNow;

				await _unitOfWork.Ratings.AddAsync(entity);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<RatingResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<RatingResponse>(entity),
					Message = "Create rating successfully."
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<RatingResponse>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<RatingResponse>> UpdateAsync(Guid id, UpdateRatingRequest request)
		{
			try
			{
				var entity = await _unitOfWork.Ratings.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<RatingResponse>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Rating not found."
					};
				}

				entity.Star = request.Star;
				entity.Comment = request.Comment;

				_unitOfWork.Ratings.UpdateAsync(entity);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<RatingResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<RatingResponse>(entity),
					Message = "Update rating successfully."
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<RatingResponse>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
		{
			try
			{
				var entity = await _unitOfWork.Ratings.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<bool>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Rating not found."
					};
				}

				_unitOfWork.Ratings.RemoveByIdAsync(id);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<bool>
				{
					IsSuccess = true,
					Data = true,
					Message = "Delete rating successfully."
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<bool>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}
	}
}

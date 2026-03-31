using AccessoriesShop.Application.IRepositories;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
	public class PromotionService : IPromotionService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public PromotionService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<ServiceResult<IEnumerable<PromotionResponse>>> GetAllAsync()
		{
			try
			{
				var entities = await _unitOfWork.Promotions.GetAllAsync(x => true);
				return new ServiceResult<IEnumerable<PromotionResponse>>
				{
					IsSuccess = true,
					Data = _mapper.Map<IEnumerable<PromotionResponse>>(entities)
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<IEnumerable<PromotionResponse>>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<PromotionResponse>> GetByIdAsync(Guid id)
		{
			try
			{
				var entity = await _unitOfWork.Promotions.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<PromotionResponse>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Promotion not found."
					};
				}

				return new ServiceResult<PromotionResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<PromotionResponse>(entity)
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<PromotionResponse>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<PromotionResponse>> CreateAsync(CreatePromotionRequest request)
		{
			try
			{
				var entity = _mapper.Map<Promotion>(request);
				entity.Id = Guid.NewGuid();

				await _unitOfWork.Promotions.AddAsync(entity);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<PromotionResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<PromotionResponse>(entity),
					Message = "Create promotion successfully."
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<PromotionResponse>
				{
					IsSuccess = false,
					Message = ex.Message
				};
			}
		}

		public async Task<ServiceResult<PromotionResponse>> UpdateAsync(Guid id, UpdatePromotionRequest request)
		{
			try
			{
				var entity = await _unitOfWork.Promotions.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<PromotionResponse>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Promotion not found."
					};
				}

				entity.Name = request.Name;
				entity.DiscountValue = request.DiscountValue;
				entity.IsPercentage = request.IsPercentage;
				entity.StartDate = request.StartDate;
				entity.EndDate = request.EndDate;
				entity.IsActive = request.IsActive;

				_unitOfWork.Promotions.UpdateAsync(entity);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<PromotionResponse>
				{
					IsSuccess = true,
					Data = _mapper.Map<PromotionResponse>(entity),
					Message = "Update promotion successfully."
				};
			}
			catch (Exception ex)
			{
				return new ServiceResult<PromotionResponse>
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
				var entity = await _unitOfWork.Promotions.GetByIdAsync(id);
				if (entity == null)
				{
					return new ServiceResult<bool>
					{
						IsSuccess = false,
						IsNotFound = true,
						Message = "Promotion not found."
					};
				}

				_unitOfWork.Promotions.RemoveByIdAsync(id);
				await _unitOfWork.SaveChangesAsync();

				return new ServiceResult<bool>
				{
					IsSuccess = true,
					Data = true,
					Message = "Delete promotion successfully."
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

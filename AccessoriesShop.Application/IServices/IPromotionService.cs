using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
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
		Task<ServiceResult<IEnumerable<PromotionResponse>>> GetAllAsync();
		Task<ServiceResult<PromotionResponse>> GetByIdAsync(Guid id);
		Task<ServiceResult<PromotionResponse>> CreateAsync(CreatePromotionRequest request);
		Task<ServiceResult<PromotionResponse>> UpdateAsync(Guid id, UpdatePromotionRequest request);
		Task<ServiceResult<bool>> DeleteAsync(Guid id);
	}
}

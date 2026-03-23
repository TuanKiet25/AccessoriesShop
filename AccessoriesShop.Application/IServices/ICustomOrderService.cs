using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface ICustomOrderService
    {
        Task<ServiceResult<CustomOrderResponse>> CreateAsync(CreateCustomOrderRequest request);
        Task<ServiceResult<CustomOrderResponse>> QuoteAsync(Guid id, QuoteCustomOrderRequest request);
        Task<ServiceResult<CustomOrderResponse>> UpdateStatusAsync(Guid id, UpdateCustomOrderStatusRequest request);
        Task<ServiceResult<CustomOrderResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<CustomOrderResponse>>> GetAllAsync();
    }
}

using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IProductCompatibilityService
    {
        Task<ServiceResult<ProductCompatibilityResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<ProductCompatibilityResponse>>> GetAllAsync();
        Task<ServiceResult<ProductCompatibilityResponse>> CreateAsync(CreateProductCompatibilityRequest request);
        Task<ServiceResult<ProductCompatibilityResponse>> UpdateAsync(Guid id, CreateProductCompatibilityRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

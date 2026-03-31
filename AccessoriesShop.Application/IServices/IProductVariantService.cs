using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.Interfaces.Services
{
    public interface IProductVariantService
    {
        Task<ServiceResult<ProductVariantResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<ProductVariantResponse>>> GetAllAsync();
        Task<ServiceResult<ProductVariantResponse>> CreateAsync(CreateProductVariantRequest request);
        Task<ServiceResult<ProductVariantResponse>> UpdateAsync(Guid id, CreateProductVariantRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IProductAttributeService
    {
        Task<ServiceResult<ProductAttributeResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<ProductAttributeResponse>>> GetAllAsync();
        Task<ServiceResult<ProductAttributeResponse>> CreateAsync(CreateProductAttributeRequest request);
        Task<ServiceResult<ProductAttributeResponse>> UpdateAsync(Guid id, CreateProductAttributeRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

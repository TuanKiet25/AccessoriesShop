using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IProductService
    {
        Task<ServiceResult<ProductResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<ProductResponse>>> GetAllAsync();
        Task<ServiceResult<ProductResponse>> CreateAsync(CreateProductRequest request);
        Task<ServiceResult<ProductResponse>> UpdateAsync(Guid id, CreateProductRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

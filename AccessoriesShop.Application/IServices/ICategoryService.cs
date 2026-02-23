using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface ICategoryService
    {
        Task<ServiceResult<CategoryResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<CategoryResponse>>> GetAllAsync();
        Task<ServiceResult<CategoryResponse>> CreateAsync(CreateCategoryRequest request);
        Task<ServiceResult<CategoryResponse>> UpdateAsync(Guid id, CreateCategoryRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

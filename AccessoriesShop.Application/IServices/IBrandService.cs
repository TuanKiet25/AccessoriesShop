using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IBrandService
    {
        Task<ServiceResult<BrandResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<BrandResponse>>> GetAllAsync();
        Task<ServiceResult<BrandResponse>> CreateAsync(CreateBrandRequest request);
        Task<ServiceResult<BrandResponse>> UpdateAsync(Guid id, CreateBrandRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

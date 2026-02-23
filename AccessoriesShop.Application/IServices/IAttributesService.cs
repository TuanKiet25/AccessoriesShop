using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IAttributesService
    {
        Task<ServiceResult<AttributesResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<AttributesResponse>>> GetAllAsync();
        Task<ServiceResult<AttributesResponse>> CreateAsync(CreateAttributesRequest request);
        Task<ServiceResult<AttributesResponse>> UpdateAsync(Guid id, CreateAttributesRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

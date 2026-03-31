using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IAddressService
    {
        Task<ServiceResult<UserAddressResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<UserAddressResponse>>> GetAllAsync();
        Task<ServiceResult<List<UserAddressResponse>>> GetByAccountIdAsync(Guid accountId);
        Task<ServiceResult<UserAddressResponse>> CreateAsync(CreateAddressRequest request);
        Task<ServiceResult<UserAddressResponse>> UpdateAsync(Guid id, CreateAddressRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

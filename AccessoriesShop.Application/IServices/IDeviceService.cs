using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IDeviceService
    {
        Task<ServiceResult<DeviceResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<DeviceResponse>>> GetAllAsync();
        Task<ServiceResult<DeviceResponse>> CreateAsync(CreateDeviceRequest request);
        Task<ServiceResult<DeviceResponse>> UpdateAsync(Guid id, CreateDeviceRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class DeviceService : IDeviceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeviceService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<DeviceResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Devices.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<DeviceResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Device not found."
                    };
                }
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<DeviceResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<DeviceResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Devices.GetAllAsync(null);
                return new ServiceResult<List<DeviceResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<DeviceResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<DeviceResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<DeviceResponse>> CreateAsync(CreateDeviceRequest request)
        {
            try
            {
                var entity = _mapper.Map<Device>(request);
                await _unitOfWork.Devices.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<DeviceResponse>(entity),
                    Message = "Device created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<DeviceResponse>> UpdateAsync(Guid id, CreateDeviceRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Devices.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<DeviceResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Device not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.Devices.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<DeviceResponse>(entity),
                    Message = "Device updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<DeviceResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<string>> DeleteAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Devices.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Device not found."
                    };
                }
                await _unitOfWork.Devices.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Device deleted successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

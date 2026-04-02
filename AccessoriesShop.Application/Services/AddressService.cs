using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILocationService _locationService;
        public AddressService(IUnitOfWork unitOfWork, IMapper mapper, ILocationService locationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _locationService = locationService;
        }

        public async Task<ServiceResult<UserAddressResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Addresses.GetAsync(e => e.Id == id && !e.isDeleted);
                if (entity == null)
                {
                    return new ServiceResult<UserAddressResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Address not found."
                    };
                }
                var response = _mapper.Map<UserAddressResponse>(entity);
                var locationNames = _locationService.GetLocationNames(entity.ProvinceCode, entity.DistrictCode, entity.WardCode);
                response.ProvinceName = locationNames.ProvinceName;
                response.DistrictName = locationNames.DistrictName;
                response.WardName = locationNames.WardName;
                return new ServiceResult<UserAddressResponse>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserAddressResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<UserAddressResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Addresses.GetAllAsync(e => !e.isDeleted);
                var responseList = _mapper.Map<List<UserAddressResponse>>(entities);
                foreach (var response in responseList)
                {
                    var locationNames = _locationService.GetLocationNames(response.ProvinceCode, response.DistrictCode, response.WardCode);
                    response.ProvinceName = locationNames.ProvinceName;
                    response.DistrictName = locationNames.DistrictName;
                    response.WardName = locationNames.WardName;
                }
                return new ServiceResult<List<UserAddressResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<UserAddressResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<UserAddressResponse>>> GetByAccountIdAsync(Guid accountId)
        {
            try
            {
                var entities = await _unitOfWork.Addresses.GetAllAsync(e => e.AccountId == accountId && !e.isDeleted);
                var responseList = _mapper.Map<List<UserAddressResponse>>(entities);
                foreach (var response in responseList)
                {
                    var locationNames = _locationService.GetLocationNames(response.ProvinceCode, response.DistrictCode, response.WardCode);
                    response.ProvinceName = locationNames.ProvinceName;
                    response.DistrictName = locationNames.DistrictName;
                    response.WardName = locationNames.WardName;
                }
                return new ServiceResult<List<UserAddressResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<UserAddressResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<UserAddressResponse>> CreateAsync(CreateAddressRequest request)
        {
            try
            {
                var entity = _mapper.Map<Address>(request);
                entity.IsDefault = true;
                await _unitOfWork.Addresses.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                var locationNames = _locationService.GetLocationNames(entity.ProvinceCode, entity.DistrictCode, entity.WardCode);
                var response = _mapper.Map<UserAddressResponse>(entity);
                response.ProvinceName = locationNames.ProvinceName;
                response.DistrictName = locationNames.DistrictName;
                response.WardName = locationNames.WardName;
                return new ServiceResult<UserAddressResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Address created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserAddressResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<UserAddressResponse>> UpdateAsync(Guid id, CreateAddressRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Addresses.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<UserAddressResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Address not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.Addresses.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                var response = _mapper.Map<UserAddressResponse>(entity);
                var locationNames = _locationService.GetLocationNames(entity.ProvinceCode, entity.DistrictCode, entity.WardCode);
                response.ProvinceName = locationNames.ProvinceName;
                response.DistrictName = locationNames.DistrictName;
                response.WardName = locationNames.WardName;
                return new ServiceResult<UserAddressResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Address updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<UserAddressResponse>
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
                var entity = await _unitOfWork.Addresses.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Address not found."
                    };
                }
                await _unitOfWork.Addresses.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Address deleted successfully."
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

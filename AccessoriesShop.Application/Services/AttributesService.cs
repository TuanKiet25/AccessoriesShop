using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class AttributesService : IAttributesService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AttributesService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AttributesResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Attributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<AttributesResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Attribute not found."
                    };
                }
                return new ServiceResult<AttributesResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AttributesResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AttributesResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<AttributesResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Attributes.GetAllAsync(null);
                return new ServiceResult<List<AttributesResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<AttributesResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<AttributesResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<AttributesResponse>> CreateAsync(CreateAttributesRequest request)
        {
            try
            {
                var entity = _mapper.Map<Attributes>(request);
                await _unitOfWork.Attributes.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<AttributesResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AttributesResponse>(entity),
                    Message = "Attribute created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AttributesResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<AttributesResponse>> UpdateAsync(Guid id, CreateAttributesRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Attributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<AttributesResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Attribute not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.Attributes.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<AttributesResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AttributesResponse>(entity),
                    Message = "Attribute updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AttributesResponse>
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
                var entity = await _unitOfWork.Attributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Attribute not found."
                    };
                }
                await _unitOfWork.Attributes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Attribute deleted successfully."
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

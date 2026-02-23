using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class ProductCompatibilityService : IProductCompatibilityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductCompatibilityService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ProductCompatibilityResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.ProductCompatibilities.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductCompatibilityResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductCompatibility not found."
                    };
                }
                return new ServiceResult<ProductCompatibilityResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductCompatibilityResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductCompatibilityResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<ProductCompatibilityResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.ProductCompatibilities.GetAllAsync(null);
                return new ServiceResult<List<ProductCompatibilityResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<ProductCompatibilityResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<ProductCompatibilityResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductCompatibilityResponse>> CreateAsync(CreateProductCompatibilityRequest request)
        {
            try
            {
                var entity = _mapper.Map<ProductCompatibility>(request);
                await _unitOfWork.ProductCompatibilities.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<ProductCompatibilityResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductCompatibilityResponse>(entity),
                    Message = "ProductCompatibility created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductCompatibilityResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductCompatibilityResponse>> UpdateAsync(Guid id, CreateProductCompatibilityRequest request)
        {
            try
            {
                var entity = await _unitOfWork.ProductCompatibilities.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductCompatibilityResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductCompatibility not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.ProductCompatibilities.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<ProductCompatibilityResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductCompatibilityResponse>(entity),
                    Message = "ProductCompatibility updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductCompatibilityResponse>
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
                var entity = await _unitOfWork.ProductCompatibilities.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductCompatibility not found."
                    };
                }
                await _unitOfWork.ProductCompatibilities.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "ProductCompatibility deleted successfully."
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

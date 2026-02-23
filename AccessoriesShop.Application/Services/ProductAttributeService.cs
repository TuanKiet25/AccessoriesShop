using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class ProductAttributeService : IProductAttributeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductAttributeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ProductAttributeResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.ProductAttributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductAttributeResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductAttribute not found."
                    };
                }
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductAttributeResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<ProductAttributeResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.ProductAttributes.GetAllAsync(null);
                return new ServiceResult<List<ProductAttributeResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<ProductAttributeResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<ProductAttributeResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductAttributeResponse>> CreateAsync(CreateProductAttributeRequest request)
        {
            try
            {
                var entity = _mapper.Map<ProductAttribute>(request);
                await _unitOfWork.ProductAttributes.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductAttributeResponse>(entity),
                    Message = "ProductAttribute created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductAttributeResponse>> UpdateAsync(Guid id, CreateProductAttributeRequest request)
        {
            try
            {
                var entity = await _unitOfWork.ProductAttributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductAttributeResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductAttribute not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.ProductAttributes.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductAttributeResponse>(entity),
                    Message = "ProductAttribute updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductAttributeResponse>
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
                var entity = await _unitOfWork.ProductAttributes.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductAttribute not found."
                    };
                }
                await _unitOfWork.ProductAttributes.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "ProductAttribute deleted successfully."
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

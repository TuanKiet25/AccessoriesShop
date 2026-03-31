using AccessoriesShop.Application.Interfaces;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

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
                var response = _mapper.Map<ProductAttributeResponse>(entity);
                
                // Manually map ProductName from Product by ProductId
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                // Manually map AttributeName from Attribute by AttributeId
                var attribute = await _unitOfWork.Attributes.GetByIdAsync(response.AttributeId);
                if (attribute != null)
                {
                    response.AttributeName = attribute.Name;
                }
                
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = response
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
                var entities = await _unitOfWork.ProductAttributes.GetAllAsync(null, include:
                                                                                q => q.Include(pa => pa.Product)
                                                                                .Include(pa => pa.Attribute));

                var responses = _mapper.Map<List<ProductAttributeResponse>>(entities); 
                return new ServiceResult<List<ProductAttributeResponse>>
                {
                    IsSuccess = true,
                    Data = responses
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
                
                var response = _mapper.Map<ProductAttributeResponse>(entity);
                
                // Manually map ProductName from Product by ProductId
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                // Manually map AttributeName from Attribute by AttributeId
                var attribute = await _unitOfWork.Attributes.GetByIdAsync(response.AttributeId);
                if (attribute != null)
                {
                    response.AttributeName = attribute.Name;
                }
                
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = response,
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
                
                var response = _mapper.Map<ProductAttributeResponse>(entity);
                
                // Manually map ProductName from Product by ProductId
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                // Manually map AttributeName from Attribute by AttributeId
                var attribute = await _unitOfWork.Attributes.GetByIdAsync(response.AttributeId);
                if (attribute != null)
                {
                    response.AttributeName = attribute.Name;
                }
                
                return new ServiceResult<ProductAttributeResponse>
                {
                    IsSuccess = true,
                    Data = response,
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

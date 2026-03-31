using AccessoriesShop.Application.Interfaces;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class ProductVariantService : IProductVariantService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductVariantService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ProductVariantResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.ProductVariants.GetAsync(e => e.Id == id && !e.isDeleted);
                if (entity == null)
                {
                    return new ServiceResult<ProductVariantResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductVariant not found."
                    };
                }
                var response = _mapper.Map<ProductVariantResponse>(entity);
                
                // Manually map ProductName from Product
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                return new ServiceResult<ProductVariantResponse>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductVariantResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<ProductVariantResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.ProductVariants.GetAllAsync(e => !e.isDeleted);
                var responses = _mapper.Map<List<ProductVariantResponse>>(entities);
                
                // Manually map ProductName for each item
                foreach (var response in responses)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                    if (product != null)
                    {
                        response.ProductName = product.Name;
                    }
                }
                
                return new ServiceResult<List<ProductVariantResponse>>
                {
                    IsSuccess = true,
                    Data = responses
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<ProductVariantResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductVariantResponse>> CreateAsync(CreateProductVariantRequest request)
        {
            try
            {
                var entity = _mapper.Map<ProductVariant>(request);
                await _unitOfWork.ProductVariants.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                
                var response = _mapper.Map<ProductVariantResponse>(entity);
                
                // Manually map ProductName from Product
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                return new ServiceResult<ProductVariantResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "ProductVariant created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductVariantResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductVariantResponse>> UpdateAsync(Guid id, CreateProductVariantRequest request)
        {
            try
            {
                var entity = await _unitOfWork.ProductVariants.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductVariantResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductVariant not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.ProductVariants.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                
                var response = _mapper.Map<ProductVariantResponse>(entity);
                
                // Manually map ProductName from Product
                var product = await _unitOfWork.Products.GetByIdAsync(response.ProductId);
                if (product != null)
                {
                    response.ProductName = product.Name;
                }
                
                return new ServiceResult<ProductVariantResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "ProductVariant updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductVariantResponse>
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
                var entity = await _unitOfWork.ProductVariants.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "ProductVariant not found."
                    };
                }
                await _unitOfWork.ProductVariants.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "ProductVariant deleted successfully."
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

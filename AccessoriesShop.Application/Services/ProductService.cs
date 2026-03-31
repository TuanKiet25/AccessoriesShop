using AccessoriesShop.Application.Interfaces;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AccessoriesShop.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<ProductResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Products.GetAsync(p => p.Id == id && !p.isDeleted, include: 
                    e => e.Include(p => p.Variants)
                          .Include(p => p.productAttributes).ThenInclude(P => P.Attribute)
                          .Include(p => p.productCompatibilities).ThenInclude(p => p.Device));
                if (entity == null)
                {
                    return new ServiceResult<ProductResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Product not found."
                    };
                }
                var brand = await _unitOfWork.Brands.GetByIdAsync(entity.BrandId);
                var category = await _unitOfWork.Categories.GetByIdAsync(entity.CategoryId);
                if (brand == null || category == null)
                {
                    return new ServiceResult<ProductResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid BrandId or CategoryId."
                    };
                }
                var response = _mapper.Map<ProductResponse>(entity);
                response.BrandName = brand.Name;
                response.CategoryName = category.Name;
                return new ServiceResult<ProductResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<ProductResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Products.GetAllAsync(e => !e.isDeleted, include:
                    e => e.Include(p => p.Variants)
                          .Include(p => p.productAttributes).ThenInclude(P => P.Attribute)
                          .Include(p => p.productCompatibilities).ThenInclude(p => p.Device));
                var responseList = new List<ProductResponse>();
                foreach (var entity in entities)
                {
                    var brand = await _unitOfWork.Brands.GetByIdAsync(entity.BrandId);
                    var category = await _unitOfWork.Categories.GetByIdAsync(entity.CategoryId);
                    if (brand != null && category != null)
                    {
                        var response = _mapper.Map<ProductResponse>(entity);
                        response.BrandName = brand.Name;
                        response.CategoryName = category.Name;
                        responseList.Add(response);
                    }
                    else
                    {
                        return new ServiceResult<List<ProductResponse>>
                        {
                            IsSuccess = false,
                            Message = "One or more products have invalid BrandId or CategoryId."
                        };
                    }
                }
                return new ServiceResult<List<ProductResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            
            catch (Exception ex)
            {
                return new ServiceResult<List<ProductResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductResponse>> CreateAsync(CreateProductRequest request)
        {
            try
            {
                var entity = _mapper.Map<Product>(request);
                await _unitOfWork.Products.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId);
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (brand == null || category == null)
                {
                    return new ServiceResult<ProductResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid BrandId or CategoryId."
                    };
                }
                var entityResult = await _unitOfWork.Products.GetAsync(e => e.Id == entity.Id, include:
                    q => q.Include(p => p.Variants)
                          .Include(p => p.productAttributes).ThenInclude(P => P.Attribute)
                          .Include(p => p.productCompatibilities).ThenInclude(p => p.Device));
                var response = _mapper.Map<ProductResponse>(entityResult);
                response.BrandName = brand.Name;
                response.CategoryName = category.Name;
                return new ServiceResult<ProductResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Product created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<ProductResponse>> UpdateAsync(Guid id, CreateProductRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Products.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<ProductResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Product not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.SaveChangesAsync();
                var brand = await _unitOfWork.Brands.GetByIdAsync(request.BrandId);
                var category = await _unitOfWork.Categories.GetByIdAsync(request.CategoryId);
                if (brand == null || category == null)
                {
                    return new ServiceResult<ProductResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid BrandId or CategoryId."
                    };
                }
                var entityResult = await _unitOfWork.Products.GetAsync(e => e.Id == entity.Id, include:
                    q => q.Include(p => p.Variants)
                          .Include(p => p.productAttributes).ThenInclude(P => P.Attribute)
                          .Include(p => p.productCompatibilities).ThenInclude(p => p.Device));
                var response = _mapper.Map<ProductResponse>(entity);
                response.BrandName = brand.Name;
                response.CategoryName = category.Name;
                return new ServiceResult<ProductResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<ProductResponse>(entity),
                    Message = "Product updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<ProductResponse>
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
                var entity = await _unitOfWork.Products.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Product not found."
                    };
                }
                await _unitOfWork.Products.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Product deleted successfully."
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

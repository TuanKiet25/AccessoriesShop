using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CategoryResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Categories.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<CategoryResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Category not found."
                    };
                }
                return new ServiceResult<CategoryResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CategoryResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CategoryResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<CategoryResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Categories.GetAllAsync(null);
                return new ServiceResult<List<CategoryResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<CategoryResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CategoryResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CategoryResponse>> CreateAsync(CreateCategoryRequest request)
        {
            try
            {
                var entity = _mapper.Map<Category>(request);
                await _unitOfWork.Categories.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<CategoryResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CategoryResponse>(entity),
                    Message = "Category created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CategoryResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CategoryResponse>> UpdateAsync(Guid id, CreateCategoryRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Categories.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<CategoryResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Category not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.Categories.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<CategoryResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CategoryResponse>(entity),
                    Message = "Category updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CategoryResponse>
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
                var entity = await _unitOfWork.Categories.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Category not found."
                    };
                }
                await _unitOfWork.Categories.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Category deleted successfully."
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

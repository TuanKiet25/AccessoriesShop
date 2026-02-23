using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;

namespace AccessoriesShop.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BrandService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<BrandResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Brands.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<BrandResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Brand not found."
                    };
                }
                return new ServiceResult<BrandResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<BrandResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<BrandResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<BrandResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Brands.GetAllAsync(null);
                return new ServiceResult<List<BrandResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<BrandResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<BrandResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<BrandResponse>> CreateAsync(CreateBrandRequest request)
        {
            try
            {
                var entity = _mapper.Map<Brand>(request);
                await _unitOfWork.Brands.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<BrandResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<BrandResponse>(entity),
                    Message = "Brand created successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<BrandResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<BrandResponse>> UpdateAsync(Guid id, CreateBrandRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Brands.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<BrandResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Brand not found."
                    };
                }
                _mapper.Map(request, entity);
                await _unitOfWork.Brands.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<BrandResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<BrandResponse>(entity),
                    Message = "Brand updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<BrandResponse>
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
                var entity = await _unitOfWork.Brands.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Brand not found."
                    };
                }
                await _unitOfWork.Brands.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Brand deleted successfully."
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

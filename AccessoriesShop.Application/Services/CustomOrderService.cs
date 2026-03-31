using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Constants;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AccessoriesShop.Application.Services
{
    public class CustomOrderService : ICustomOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CustomOrderService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<CustomOrderResponse>> CreateAsync(CreateCustomOrderRequest request)
        {
            try
            {
                var entity = _mapper.Map<CustomOrder>(request);
                if (request.ProductBaseId.HasValue)
                {
                    var product = await _unitOfWork.Products.GetByIdAsync(request.ProductBaseId.Value);
                    if (product == null)
                    {
                        return new ServiceResult<CustomOrderResponse>
                        {
                            IsSuccess = false,
                            Message = "Invalid base product id."
                        };
                    }
                    entity.EstimatedPrice = product.Price * request.Quantity;
                }
                if (request.ImageUrls != null && request.ImageUrls.Any())
                {
                    entity.Files = request.ImageUrls.Select(url => new CustomOrderFile
                    {
                        FileUrl = url
                    }).ToList();
                }

                await _unitOfWork.CustomOrders.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var created = await _unitOfWork.CustomOrders.GetAsync(c => c.Id == entity.Id,
                    include: q => q.Include(c => c.Files));
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CustomOrderResponse>(created),
                    Message = "Custom order created."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<CustomOrderResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.CustomOrders.GetAllAsync(c => !c.isDeleted,
                    include: q => q.Include(c => c.Files));
                return new ServiceResult<List<CustomOrderResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<CustomOrderResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CustomOrderResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CustomOrderResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.CustomOrders.GetAsync(c => c.Id == id,
                    include: q => q.Include(c => c.Files));
                if (entity == null)
                {
                    return new ServiceResult<CustomOrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Custom order not found."
                    };
                }
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CustomOrderResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CustomOrderResponse>> QuoteAsync(Guid id, QuoteCustomOrderRequest request)
        {
            try
            {
                var entity = await _unitOfWork.CustomOrders.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<CustomOrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Custom order not found."
                    };
                }

                entity.FinalPrice = request.Price;
                entity.EstimatedDeliveryDate = request.EstimatedDeliveryDate;
                if (!string.IsNullOrWhiteSpace(request.Note))
                {
                    entity.Note = request.Note;
                }
                entity.Status = CustomOrderStatus.Quoted;
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CustomOrderResponse>(entity),
                    Message = "Quote updated."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CustomOrderResponse>> UpdateStatusAsync(Guid id, UpdateCustomOrderStatusRequest request)
        {
            try
            {
                var entity = await _unitOfWork.CustomOrders.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<CustomOrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Custom order not found."
                    };
                }
                if (!string.IsNullOrWhiteSpace(request.Note))
                {
                    entity.Note = request.Note;
                }
                entity.Status = request.Status;

                // If approved but no final price yet, fall back to estimated price
                if (entity.Status == CustomOrderStatus.Approved && entity.FinalPrice == null)
                {
                    entity.FinalPrice = entity.EstimatedPrice;
                }

                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CustomOrderResponse>(entity),
                    Message = "Status updated."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CustomOrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

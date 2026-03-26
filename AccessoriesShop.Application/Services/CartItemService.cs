using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class CartItemService : ICartItemService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CartItemService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResult<CartItemResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.CartItems.GetAsync(e => e.Id == id, include: q => q.Include(e => e.ProductVariant));
                if (entity == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "CartItem not found!"
                    };
                }

                var response = _mapper.Map<CartItemResponse>(entity);
                return new ServiceResult<CartItemResponse>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CartItemResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<CartItemResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.CartItems.GetAllAsync(null, include: q => q.Include(e => e.ProductVariant));
                var responseList = _mapper.Map<List<CartItemResponse>>(entities);

                return new ServiceResult<List<CartItemResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CartItemResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CartItemResponse>> CreateAsync(CartItemRequest request)
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");
                var userCart = await _unitOfWork.Carts.GetAsync(c => c.AccountId == userId);
                if(userCart == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Cart not found for user! Please create a cart first!"
                    };
                }
                var productVariant = await _unitOfWork.ProductVariants.GetByIdAsync(request.ProductVariantId);    
                if (productVariant == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "productVariant not found!"
                    };
                }
                if (request.Quantity > productVariant.StockQuantity)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        Message = $"Only {productVariant.StockQuantity} items left in stock!"
                    };
                }
                var entity = _mapper.Map<CartItem>(request);
                entity.CartId = userCart.Id;
                entity.CreateTime = DateTime.UtcNow;
                entity.UpdateTime = DateTime.UtcNow;
                
                await _unitOfWork.CartItems.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                var entityFull = await _unitOfWork.CartItems.GetAsync(e => e.Id == entity.Id, include: q => q.Include(e => e.ProductVariant));
                var response = _mapper.Map<CartItemResponse>(entityFull);
                return new ServiceResult<CartItemResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Create CartItem successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CartItemResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<CartItemResponse>> UpdateAsync(Guid id, CartItemRequest request)
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");
                var userCart = await _unitOfWork.Carts.GetAsync(c => c.AccountId == userId);
                if (userCart == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Cart not found for user! Please create a cart first!"
                    };
                }
                var productVariant = await _unitOfWork.ProductVariants.GetByIdAsync(request.ProductVariantId);
                if (productVariant == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "productVariant not found!"
                    };
                }
                if (request.Quantity > productVariant.StockQuantity)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        Message = $"Only {productVariant.StockQuantity} items left in stock!"
                    };
                }
                var entity = await _unitOfWork.CartItems.GetAsync(e => e.Id == id, include: q => q.Include(e => e.ProductVariant));
                if (entity == null)
                {
                    return new ServiceResult<CartItemResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "CartItem Not found!!!"
                    };
                }

                entity.CartId = userCart.Id;
                entity.ProductVariantId = request.ProductVariantId;
                entity.Quantity = request.Quantity;
                entity.CreateTime = DateTime.UtcNow;
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<CartItemResponse>(entity);
                return new ServiceResult<CartItemResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "CartItem Update successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CartItemResponse>
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
                var entity = await _unitOfWork.CartItems.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "CartItem không tìm thấy"
                    };
                }

                await _unitOfWork.CartItems.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Data = "Xóa CartItem thành công",
                    Message = "Xóa CartItem thành công"
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

        public async Task<ServiceResult<CartResponse>> CreateCardAsync()
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");
                var card = new Cart
                {
                    AccountId = userId,
                    CreateTime = DateTime.UtcNow,
                };
                await _unitOfWork.Carts.AddAsync(card);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<CartResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<CartResponse>(card),
                    Message = "Card create successfully!"
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<CartResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<CartItemResponse>>> GetAllByUserAsync()
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");
                var userCart = await _unitOfWork.Carts.GetAsync(c => c.AccountId == userId);
                if(userCart == null)
                {
                    return new ServiceResult<List<CartItemResponse>>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Cart not found for user! Please create a cart first!"
                    };
                }
                var entities = await _unitOfWork.CartItems.GetAllAsync(e => e.CartId == userCart.Id, include: q => q.Include(e => e.ProductVariant));
                var responseList = _mapper.Map<List<CartItemResponse>>(entities);

                return new ServiceResult<List<CartItemResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<CartItemResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

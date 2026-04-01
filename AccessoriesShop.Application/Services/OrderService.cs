using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Constants;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace AccessoriesShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IStockReservationService _stockReservationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocationService _locationService;
        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderService> logger,
            IStockReservationService stockReservationService,
            IHttpContextAccessor httpContextAccessor,
            ILocationService locationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _stockReservationService = stockReservationService;
            _httpContextAccessor = httpContextAccessor;
            _locationService = locationService;
        }

        public async Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Orders.GetAsync(e => e.Id == id, include: q => q.Include(e => e.OrderItems));
                if (entity == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                var response = _mapper.Map<OrderResponse>(entity);
                if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                {
                    response.OrderItems = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
                }

                return new ServiceResult<OrderResponse> 
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting order by id: {ex.Message}");
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<OrderResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Orders.GetAllAsync(null, include : q => q.Include(e => e.OrderItems));

                var responseList = new List<OrderResponse>();

                foreach (var entity in entities)
                {
                    var response = _mapper.Map<OrderResponse>(entity);
                    if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                    {
                        response.OrderItems = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
                    }
                    responseList.Add(response);
                }

                return new ServiceResult<List<OrderResponse>>
                {
                    IsSuccess = true,
                    Data = responseList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all orders: {ex.Message}");
                return new ServiceResult<List<OrderResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<OrderResponse>> CreateAsync(CreateOrderRequest request)
        {
            try
            {
                // Validate that OrderItems exist
                if (request.OrderItems == null || request.OrderItems.Count == 0)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Order must contain at least one item."
                    };
                }

                // Verify that the account exists
                var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
                if (account == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid AccountId. Account does not exist."
                    };
                }

                // Create the Order entity from the request
                var entity = _mapper.Map<Order>(request);
                //Handle shipping detail 
                var userAddress = await _unitOfWork.Addresses.GetAsync(ua => ua.Id == request.AddressId && ua.AccountId == request.AccountId && ua.IsDefault);
                if (userAddress == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid AddressId. Address does not exist or is not default."
                    };
                }
                var locationNames = _locationService.GetLocationNames(userAddress.ProvinceCode, userAddress.DistrictCode, userAddress.WardCode);
                var snapshotAddress = new
                {
                    ProvinceName = locationNames.ProvinceName,
                    DistrictName = locationNames.DistrictName,
                    WardName = locationNames.WardName,
                    StreetAddress = userAddress.StreetAddress,
                    ReceiverName = string.IsNullOrWhiteSpace(request.ReceiverName)
                   ? account.Username
                   : request.ReceiverName,
                    ReceiverPhone = string.IsNullOrWhiteSpace(request.ReceiverPhone)
                    ? account.PhoneNumber
                    : request.ReceiverPhone
                };
                entity.ShippingDetail = JsonSerializer.Serialize(snapshotAddress);
                // Fetch variant details and create OrderItems with prices from database
                entity.OrderItems = new List<OrderItem>();
                
                foreach (var itemRequest in request.OrderItems)
                {
                    // Fetch the variant from database to get the current price
                    var variant = await _unitOfWork.ProductVariants.GetByIdAsync(itemRequest.VariantId);
                    if (variant == null)
                    {
                        return new ServiceResult<OrderResponse>
                        {
                            IsSuccess = false,
                            Message = $"Product variant with ID {itemRequest.VariantId} not found."
                        };
                    }

                    // Check if there's enough stock
                    if (variant.StockQuantity < itemRequest.Quantity)
                    {
                        return new ServiceResult<OrderResponse>
                        {
                            IsSuccess = false,
                            Message = $"Insufficient stock for variant {variant.Name}. Available: {variant.StockQuantity}, Requested: {itemRequest.Quantity}"
                        };
                    }

                    // Apply promotion if active
                    decimal finalPrice = variant.Price;
                    var promotions = await _unitOfWork.Promotions.GetAllAsync(p => p.ProductId == variant.ProductId && p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);
                    if (promotions != null && promotions.Any())
                    {
                        var bestPromotion = promotions.OrderByDescending(p => p.IsPercentage ? (variant.Price * p.DiscountValue / 100) : p.DiscountValue).First();
                        if (bestPromotion.IsPercentage)
                        {
                            finalPrice = finalPrice - (finalPrice * bestPromotion.DiscountValue / 100);
                        }
                        else
                        {
                            finalPrice = finalPrice - bestPromotion.DiscountValue;
                        }
                        if (finalPrice < 0) finalPrice = 0;
                    }

                    // Create OrderItem with price fetched from variant and apply discount
                    var orderItem = new OrderItem
                    {
                        OrderId = entity.Id,
                        VariantId = itemRequest.VariantId,
                        Quantity = itemRequest.Quantity,
                        Price = finalPrice  // Get price from database + apply discount
                    };

                    entity.OrderItems.Add(orderItem);
                }

                // Automatically calculate TotalAmount from OrderItems
                entity.CalculateTotalAmount();

                // Ensure status is set
                if (string.IsNullOrEmpty(entity.Status))
                {
                    entity.Status = OrderStatus.Pending;
                }

                // Add the order with its items
                await _unitOfWork.Orders.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Reserve stock for the order items
                var stockReservationResult = await _stockReservationService.ReserveStockAsync(entity.Id);
                if (!stockReservationResult.IsSuccess)
                {
                    // Stock reservation failed - delete the order
                    await _unitOfWork.Orders.DeleteAsync(entity.Id);
                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogWarning($"Stock reservation failed for order {entity.Id}. Order deleted. Reason: {stockReservationResult.Message}");
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = $"Order creation failed: {stockReservationResult.Message}"
                    };
                }

                // Reload the order to get all related data
                var createdOrder = await _unitOfWork.Orders.GetByIdAsync(entity.Id);
                var response = _mapper.Map<OrderResponse>(createdOrder);
                if (createdOrder.OrderItems != null && createdOrder.OrderItems.Count > 0)
                {
                    response.OrderItems = _mapper.Map<List<OrderItemResponse>>(createdOrder.OrderItems);
                }

                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = ApiMessages.Order.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order: {ex.Message}");
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<OrderResponse>> UpdateAsync(Guid id, CreateOrderRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Orders.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                // Verify that the account exists if being updated
                if (entity.AccountId != request.AccountId)
                {
                    var account = await _unitOfWork.Accounts.GetByIdAsync(request.AccountId);
                    if (account == null)
                    {
                         return new ServiceResult<OrderResponse>
                        {
                            IsSuccess = false,
                            Message = "Invalid AccountId. Account does not exist."
                        };
                    }
                }

                // Update order properties
                entity.OrderDate = DateTime.UtcNow; // Update order date to current time
                entity.Status = OrderStatus.Pending;
                entity.AccountId = request.AccountId;

                // Update OrderItems if provided
                if (request.OrderItems != null && request.OrderItems.Count > 0)
                {
                    entity.OrderItems = new List<OrderItem>();
                    
                    foreach (var itemRequest in request.OrderItems)
                    {
                        // Fetch the variant from database to get the current price
                        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(itemRequest.VariantId);
                        if (variant == null)
                        {
                            return new ServiceResult<OrderResponse>
                            {
                                IsSuccess = false,
                                Message = $"Product variant with ID {itemRequest.VariantId} not found."
                            };
                        }

                        // Apply promotion if active
                        decimal finalPrice = variant.Price;
                        var promotions = await _unitOfWork.Promotions.GetAllAsync(p => p.ProductId == variant.ProductId && p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);
                        if (promotions != null && promotions.Any())
                        {
                            var bestPromotion = promotions.OrderByDescending(p => p.IsPercentage ? (variant.Price * p.DiscountValue / 100) : p.DiscountValue).First();
                            if (bestPromotion.IsPercentage)
                            {
                                finalPrice = finalPrice - (finalPrice * bestPromotion.DiscountValue / 100);
                            }
                            else
                            {
                                finalPrice = finalPrice - bestPromotion.DiscountValue;
                            }
                            if (finalPrice < 0) finalPrice = 0;
                        }

                        // Create OrderItem with price fetched from variant and apply discount
                        var orderItem = new OrderItem
                        {
                            OrderId = entity.Id,
                            VariantId = itemRequest.VariantId,
                            Quantity = itemRequest.Quantity,
                            Price = finalPrice  // Get price from database + apply discount
                        };

                        entity.OrderItems.Add(orderItem);
                    }
                }

                // Automatically recalculate TotalAmount from OrderItems
                entity.CalculateTotalAmount();

                await _unitOfWork.Orders.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<OrderResponse>(entity);
                if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                {
                    response.OrderItems = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
                }

                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = ApiMessages.Order.Updated
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating order: {ex.Message}");
                return new ServiceResult<OrderResponse>
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
                var entity = await _unitOfWork.Orders.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                await _unitOfWork.Orders.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = ApiMessages.Order.Deleted
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting order: {ex.Message}");
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<OrderResponse>> PlaceOrderFromSelectedItemsAsync(List<Guid> cartItemIds, string? ReceiverName, string? ReceiverPhone, Guid AddressId)
        {
            try
            {
                // 1. Lấy thông tin UserId từ Token
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");

                // 2. Lấy các CartItems dựa trên danh sách ID được chọn
                var cartItems = await _unitOfWork.CartItems.GetAllAsync(
                    filter: ci => cartItemIds.Contains(ci.Id) && ci.Cart.AccountId == userId,
                    include: q => q.Include(ci => ci.ProductVariant).Include(ci => ci.Cart)
                );

                if (cartItems == null || !cartItems.Any())
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "không tồn tại sản phẩm trong giỏ hàng"
                    };
                }
                //Handle shipping detail
                var account = await _unitOfWork.Accounts.GetByIdAsync(userId);
                var userAddress = await _unitOfWork.Addresses.GetAsync(ua => ua.Id == AddressId && ua.AccountId == userId && ua.IsDefault);
                if (userAddress == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid AddressId. Address does not exist or is not default."
                    };
                }
                var locationNames = _locationService.GetLocationNames(userAddress.ProvinceCode, userAddress.DistrictCode, userAddress.WardCode);
                var snapshotAddress = new
                {
                    ProvinceName = locationNames.ProvinceName,
                    DistrictName = locationNames.DistrictName,
                    WardName = locationNames.WardName,
                    StreetAddress = userAddress.StreetAddress,
                    ReceiverName = string.IsNullOrWhiteSpace(ReceiverName)
                   ? account.Username
                   : ReceiverName,
                    ReceiverPhone = string.IsNullOrWhiteSpace(ReceiverPhone)
                    ? account.PhoneNumber
                    : ReceiverPhone
                };
                // 3. Khởi tạo thực thể Order
                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    AccountId = userId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    OrderItems = new List<OrderItem>(),
                    ShippingDetail = JsonSerializer.Serialize(snapshotAddress)
                };

                // 4. Duyệt qua các item đã chọn để kiểm tra kho và tạo OrderItem
                foreach (var cartItem in cartItems)
                {
                    var variant = cartItem.ProductVariant;

                    if (variant == null || variant.StockQuantity < cartItem.Quantity)
                    {
                        return new ServiceResult<OrderResponse>
                        {
                            IsSuccess = false,
                            Message = $"Sản phẩm '{variant?.Name}' không đủ hàng tồn kho."
                        };
                    }

                    // Apply promotion if active
                    decimal finalPrice = variant.Price;
                    var promotions = await _unitOfWork.Promotions.GetAllAsync(p => p.ProductId == variant.ProductId && p.IsActive && p.StartDate <= DateTime.UtcNow && p.EndDate >= DateTime.UtcNow);
                    if (promotions != null && promotions.Any())
                    {
                        var bestPromotion = promotions.OrderByDescending(p => p.IsPercentage ? (variant.Price * p.DiscountValue / 100) : p.DiscountValue).First();
                        if (bestPromotion.IsPercentage)
                        {
                            finalPrice = finalPrice - (finalPrice * bestPromotion.DiscountValue / 100);
                        }
                        else
                        {
                            finalPrice = finalPrice - bestPromotion.DiscountValue;
                        }
                        if (finalPrice < 0) finalPrice = 0;
                    }

                    order.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.Id,
                        VariantId = cartItem.ProductVariantId,
                        Quantity = cartItem.Quantity,
                        Price = finalPrice // Chốt giá tại thời điểm mua sau khi áp dụng mã giảm giá
                    });
                }

                // 5. Tính tổng tiền và lưu Order
                order.CalculateTotalAmount();
                await _unitOfWork.Orders.AddAsync(order);

                // 6. Xóa các CartItem đã được chọn ra khỏi giỏ hàng
                _unitOfWork.CartItems.RemoveRange(cartItems);

                // 7. Lưu thay đổi tạm thời xuống DB
                await _unitOfWork.SaveChangesAsync();

                // 8. Gọi dịch vụ giữ chỗ kho (Trừ StockQuantity)
                var stockReservationResult = await _stockReservationService.ReserveStockAsync(order.Id);
                if (!stockReservationResult.IsSuccess)
                {
                    await _unitOfWork.Orders.DeleteAsync(order.Id);
                    await _unitOfWork.SaveChangesAsync();
                    _logger.LogWarning($"Stock reservation failed for order {order.Id}. Order deleted. Reason: {stockReservationResult.Message}");
                    return new ServiceResult<OrderResponse>

                    {
                        IsSuccess = false,
                        Message = $"Order creation failed: {stockReservationResult.Message}"
                    };
                }
                var createdOrder = await _unitOfWork.Orders.GetAsync(o => o.Id == order.Id, include: q => q.Include(o => o.OrderItems));
                var response = _mapper.Map<OrderResponse>(createdOrder);
                if (createdOrder.OrderItems != null && createdOrder.OrderItems.Count > 0)
                {
                    response.OrderItems = _mapper.Map<List<OrderItemResponse>>(createdOrder.OrderItems);

                }
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = ApiMessages.Order.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating order: {ex.Message}");
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<OrderResponse>> UpdateShippingDetailAsync(Guid orderId, string? ReceiverName, string? ReceiverPhone, Guid AddressId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId); 
                if(order == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }
                var account = await _unitOfWork.Accounts.GetByIdAsync(order.AccountId);
                var userAddress = await _unitOfWork.Addresses.GetAsync(ua => ua.Id == AddressId && ua.AccountId == account.Id && ua.IsDefault);
                if (userAddress == null)
                {
                    return new ServiceResult<OrderResponse>
                    {
                        IsSuccess = false,
                        Message = "Invalid AddressId. Address does not exist or is not default."
                    };
                }
                var locationNames = _locationService.GetLocationNames(userAddress.ProvinceCode, userAddress.DistrictCode, userAddress.WardCode);
                var snapshotAddress = new
                {
                    ProvinceName = locationNames.ProvinceName,
                    DistrictName = locationNames.DistrictName,
                    WardName = locationNames.WardName,
                    StreetAddress = userAddress.StreetAddress,
                    ReceiverName = string.IsNullOrWhiteSpace(ReceiverName)
                   ? account.Username
                   : ReceiverName,
                    ReceiverPhone = string.IsNullOrWhiteSpace(ReceiverPhone)
                    ? account.PhoneNumber
                    : ReceiverPhone
                };
                order.ShippingDetail = JsonSerializer.Serialize(snapshotAddress);
                await _unitOfWork.Orders.UpdateAsync(order);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<OrderResponse>(order),
                    Message = "Cập nhật thông tin giao hàng thành công."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<OrderResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }    
        }
        public async Task<ServiceResult<List<OrderResponse>>> GetOrderByUserId()
        {
            try
            {
                var userIdString = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                    throw new Exception("Invalid ID from token");
                var orders = await _unitOfWork.Orders.GetAllAsync(filter: o => o.AccountId == userId, include: q => q.Include(o => o.OrderItems));
                var results = _mapper.Map<List<OrderResponse>>(orders);
                return new ServiceResult<List<OrderResponse>>
                {
                    IsSuccess = true,
                    Data = results,
                    Message = "Lấy danh sách đơn hàng của người dùng thành công."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<OrderResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Constants;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OrderService> _logger;
        private readonly IStockReservationService _stockReservationService;

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderService> logger,
            IStockReservationService stockReservationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _stockReservationService = stockReservationService;
        }

        public async Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id)
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

                var response = _mapper.Map<OrderResponse>(entity);
                if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                {
                    response.Items = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
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
                var entities = await _unitOfWork.Orders.GetAllAsync(null);
                var responseList = new List<OrderResponse>();

                foreach (var entity in entities)
                {
                    var response = _mapper.Map<OrderResponse>(entity);
                    if (entity.OrderItems != null && entity.OrderItems.Count > 0)
                    {
                        response.Items = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
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

                    // Create OrderItem with price fetched from variant
                    var orderItem = new OrderItem
                    {
                        OrderId = entity.Id,
                        VariantId = itemRequest.VariantId,
                        Quantity = itemRequest.Quantity,
                        Price = variant.Price  // Get price from database
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
                    response.Items = _mapper.Map<List<OrderItemResponse>>(createdOrder.OrderItems);
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

                        // Create OrderItem with price fetched from variant
                        var orderItem = new OrderItem
                        {
                            OrderId = entity.Id,
                            VariantId = itemRequest.VariantId,
                            Quantity = itemRequest.Quantity,
                            Price = variant.Price  // Get price from database
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
                    response.Items = _mapper.Map<List<OrderItemResponse>>(entity.OrderItems);
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
    }
}

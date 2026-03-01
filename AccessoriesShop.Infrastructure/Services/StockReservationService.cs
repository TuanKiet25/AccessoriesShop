using AccessoriesShop.Application;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Infrastructure.Services
{
    /// <summary>
    /// Manages stock reservations when orders are created and payments are processed
    /// </summary>
    public class StockReservationService : IStockReservationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockReservationService> _logger;

        public StockReservationService(
            IUnitOfWork unitOfWork,
            ILogger<StockReservationService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// <summary>
        /// Reserve stock when order is created (reduce stock quantities)
        /// </summary>
        public async Task<ServiceResult<string>> ReserveStockAsync(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Order not found."
                    };
                }

                if (order.OrderItems == null || order.OrderItems.Count == 0)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = true,
                        Message = "No items to reserve."
                    };
                }

                // Reserve stock for each order item
                foreach (var orderItem in order.OrderItems)
                {
                    var variant = await _unitOfWork.ProductVariants.GetByIdAsync(orderItem.VariantId);
                    if (variant == null)
                    {
                        return new ServiceResult<string>
                        {
                            IsSuccess = false,
                            Message = $"Product variant {orderItem.VariantId} not found."
                        };
                    }

                    // Check if sufficient stock is available
                    if (variant.StockQuantity < orderItem.Quantity)
                    {
                        return new ServiceResult<string>
                        {
                            IsSuccess = false,
                            Message = $"Insufficient stock for variant {variant.Name}. Available: {variant.StockQuantity}, Requested: {orderItem.Quantity}"
                        };
                    }

                    // Reduce stock (reserve)
                    variant.StockQuantity -= orderItem.Quantity;
                    await _unitOfWork.ProductVariants.UpdateAsync(variant);

                    _logger.LogInformation(
                        "Stock reserved: VariantId={VariantId}, ReservedQuantity={Quantity}, RemainingStock={RemainingStock}",
                        variant.Id, orderItem.Quantity, variant.StockQuantity);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Stock reservation successful for Order {OrderId}", orderId);
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Stock reserved successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reserving stock for order {orderId}: {ex.Message}");
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Confirm stock reservation (called when payment succeeds)
        /// Stock is already reserved, so no action needed
        /// </summary>
        public async Task<ServiceResult<string>> ConfirmStockReservationAsync(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Stock reservation confirmed for Order {OrderId}", orderId);
                return await Task.FromResult(new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Stock reservation confirmed (no action needed)."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error confirming stock reservation for order {orderId}: {ex.Message}");
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        /// <summary>
        /// Revert stock reservation (called when payment fails or expires)
        /// Restore stock quantities
        /// </summary>
        public async Task<ServiceResult<string>> RevertStockReservationAsync(Guid orderId)
        {
            try
            {
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Order not found."
                    };
                }

                if (order.OrderItems == null || order.OrderItems.Count == 0)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = true,
                        Message = "No items to revert."
                    };
                }

                // Restore stock for each order item
                foreach (var orderItem in order.OrderItems)
                {
                    var variant = await _unitOfWork.ProductVariants.GetByIdAsync(orderItem.VariantId);
                    if (variant == null)
                    {
                        _logger.LogWarning("Product variant {VariantId} not found for order {OrderId}", orderItem.VariantId, orderId);
                        continue;
                    }

                    // Restore stock (revert reservation)
                    variant.StockQuantity += orderItem.Quantity;
                    await _unitOfWork.ProductVariants.UpdateAsync(variant);

                    _logger.LogInformation(
                        "Stock reverted: VariantId={VariantId}, RevertedQuantity={Quantity}, RestoredStock={RestoredStock}",
                        variant.Id, orderItem.Quantity, variant.StockQuantity);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Stock reservation reverted for Order {OrderId}", orderId);
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Stock reservation reverted successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reverting stock reservation for order {orderId}: {ex.Message}");
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

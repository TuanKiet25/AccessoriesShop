using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
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

        public OrderService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
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
                        Message = API.Order.NotFound
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

                var entity = _mapper.Map<Order>(request);
                await _unitOfWork.Orders.AddAsync(entity);
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
                    Message = "Order created successfully."
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
                        Message = "Order not found."
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

                _mapper.Map(request, entity);
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
                    Message = "Order updated successfully."
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
                        Message = "Order not found."
                    };
                }

                await _unitOfWork.Orders.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Order deleted successfully."
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

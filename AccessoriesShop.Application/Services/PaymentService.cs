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
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<PaymentResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Payments.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<PaymentResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Payment.NotFound
                    };
                }

                var response = _mapper.Map<PaymentResponse>(entity);
                return new ServiceResult<PaymentResponse>
                {
                    IsSuccess = true,
                    Data = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting payment by id: {ex.Message}");
                return new ServiceResult<PaymentResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<PaymentResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Payments.GetAllAsync(null);
                var responses = _mapper.Map<List<PaymentResponse>>(entities);

                return new ServiceResult<List<PaymentResponse>>
                {
                    IsSuccess = true,
                    Data = responses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting all payments: {ex.Message}");
                return new ServiceResult<List<PaymentResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<PaymentResponse>>> GetByOrderIdAsync(Guid orderId)
        {
            try
            {
                // Verify that the order exists
                var order = await _unitOfWork.Orders.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new ServiceResult<List<PaymentResponse>>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                var entities = await _unitOfWork.Payments.GetAllAsync(p => p.OrderId == orderId);
                var responses = _mapper.Map<List<PaymentResponse>>(entities);

                return new ServiceResult<List<PaymentResponse>>
                {
                    IsSuccess = true,
                    Data = responses
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting payments by order id: {ex.Message}");
                return new ServiceResult<List<PaymentResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<PaymentResponse>> CreateAsync(CreatePaymentRequest request)
        {
            try
            {
                // Verify that the order exists
                var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return new ServiceResult<PaymentResponse>
                    {
                        IsSuccess = false,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                // Validate amount matches order total
                if (request.Amount <= 0)
                {
                    return new ServiceResult<PaymentResponse>
                    {
                        IsSuccess = false,
                        Message = "Payment amount must be greater than zero."
                    };
                }

                var entity = _mapper.Map<Payment>(request);

                // Set default status if not provided
                if (string.IsNullOrEmpty(entity.Status))
                {
                    entity.Status = PaymentStatus.Pending;
                }

                await _unitOfWork.Payments.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<PaymentResponse>(entity);
                return new ServiceResult<PaymentResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = ApiMessages.Payment.Created
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating payment: {ex.Message}");
                return new ServiceResult<PaymentResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<PaymentResponse>> UpdateAsync(Guid id, CreatePaymentRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Payments.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<PaymentResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Payment.NotFound
                    };
                }

                // Verify that the order exists if being updated
                if (entity.OrderId != request.OrderId)
                {
                    var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                    if (order == null)
                    {
                        return new ServiceResult<PaymentResponse>
                        {
                            IsSuccess = false,
                            Message = ApiMessages.Order.NotFound
                        };
                    }
                }

                // Validate amount is valid
                if (request.Amount <= 0)
                {
                    return new ServiceResult<PaymentResponse>
                    {
                        IsSuccess = false,
                        Message = "Payment amount must be greater than zero."
                    };
                }

                // Update properties
                entity.OrderId = request.OrderId;
                entity.Amount = request.Amount;
                entity.Currency = request.Currency ?? "VND";
                entity.PaymentMethod = request.PaymentMethod;
                entity.TransactionCode = request.TransactionCode;
                entity.Status = request.Status ?? PaymentStatus.Pending;
                entity.BankCode = request.BankCode;
                entity.PaidAt = request.PaidAt;
                entity.ExpiredAt = request.ExpiredAt;
                entity.PaymentUrl = request.PaymentUrl;
                entity.ResponseCode = request.ResponseCode;
                entity.ResponseMessage = request.ResponseMessage;
                entity.TransactionRef = request.TransactionRef;

                await _unitOfWork.Payments.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                var response = _mapper.Map<PaymentResponse>(entity);
                return new ServiceResult<PaymentResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Payment updated successfully."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating payment: {ex.Message}");
                return new ServiceResult<PaymentResponse>
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
                var entity = await _unitOfWork.Payments.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Payment.NotFound
                    };
                }

                await _unitOfWork.Payments.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = ApiMessages.Payment.Deleted
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting payment: {ex.Message}");
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

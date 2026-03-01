using AccessoriesShop.Application;
using AccessoriesShop.Application.Common.Constants;
using AccessoriesShop.Application.Common.Settings;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Constants;
using AccessoriesShop.Domain.Entities;
using IdGen;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Org.BouncyCastle.Asn1.Cmp;
using PayOS;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Models.Webhooks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AccessoriesShop.Infrastructure.Services
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOSClient _payOS;
        private readonly PayOSSettings _settings;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIdGenerator<long> _idGenerator;
        private readonly ILogger<PayOSService> _logger;
        private readonly IStockReservationService _stockReservationService;

        public PayOSService(
            IOptions<PayOSSettings> settings,
            IUnitOfWork unitOfWork,
            ILogger<PayOSService> logger,
            IIdGenerator<long> idGenerator,
            IStockReservationService stockReservationService)
        {
            _settings = settings.Value;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _stockReservationService = stockReservationService;

            _payOS = new PayOSClient(new PayOSOptions
            {
                ClientId = _settings.ClientId,
                ApiKey = _settings.ApiKey,
                ChecksumKey = _settings.ChecksumKey
            });
            _idGenerator = idGenerator;
        }

        public async Task<ServiceResult<PayOSResponse>> CreatePaymentLinkAsync(PayOSRequest request)
        {
            try
            {
                // Validate order exists
                var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId);
                if (order == null)
                {
                    return new ServiceResult<PayOSResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Order.NotFound
                    };
                }

                // Generate unique orderCode (PayOS requires numeric long)
                // Limit to 8 characters for better readability
                long refCode = _idGenerator.CreateId();
                string refCodeString = Math.Abs(refCode % 100000000).ToString().PadLeft(8, '0');


                // Create payment record
                var payment = new Payment
                {
                    Currency = "VND",
                    OrderId = request.OrderId,
                    PaymentMethod = PaymentGateway.PayOS,
                    TransactionRef = refCodeString,
                    Amount = order.TotalAmount,
                    Status = PaymentStatus.Pending,
                    ExpiredAt = DateTime.UtcNow.AddMinutes(15)
                };

                await _unitOfWork.Payments.AddAsync(payment);
                await _unitOfWork.SaveChangesAsync();

                // Create PayOS payment link
                var description = $"AccessoriesShop#{refCodeString}";
                // PayOS description max 25 chars
                if (description.Length > 25)
                    description = description[..25];

                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = long.Parse(refCodeString),
                    Amount = (long)order.TotalAmount,
                    Description = description,
                    CancelUrl = _settings.CancelUrl,
                    ReturnUrl = _settings.ReturnUrl
                };

                var createPaymentResult = await _payOS.PaymentRequests.CreateAsync(paymentRequest);

                // 5. Update payment with URL
                payment.PaymentUrl = createPaymentResult.CheckoutUrl;

                _logger.LogInformation(
                    "PayOS payment link created: PaymentId={PaymentId}, OrderCode={OrderCode}",
                    payment.Id, refCodeString);

                return new ServiceResult<PayOSResponse>
                {
                    IsSuccess = true,
                    Message = "Payment link created",
                    Data = new PayOSResponse
                    {
                        Success = true,
                        PaymentUrl = createPaymentResult.CheckoutUrl,
                        QrCode = createPaymentResult.QrCode,
                        TransactionId = refCodeString

                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreatePaymentLinkAsync");
                return new ServiceResult<PayOSResponse>
                {
                    IsSuccess = false,
                    Message = ApiMessages.ServerError.General
                };
            }
        }

        public async Task<ServiceResult<ProcessPaymentResult>> ProcessCallbackAsync(string orderCode, string status)
        {
            try
            {
                _logger.LogInformation(
                    "Processing PayOS callback: OrderCode={OrderCode}, Status={Status}",
                    orderCode, status);

                // Find payment by orderCode (stored as TransactionRef)
                var payment = await _unitOfWork.Payments.GetByOrderCodeAsync(orderCode, PaymentGateway.PayOS);

                if (payment == null)
                {
                    _logger.LogWarning("PayOS payment not found for OrderCode: {OrderCode}", orderCode);
                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = ApiMessages.Payment.NotFound
                    };
                }

                // 2. Prevent double processing
                if (payment.Status == PaymentStatus.Success)
                {
                    _logger.LogInformation("PayOS payment {PaymentId} already processed as success", payment.Id);
                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = true,
                        Message = ApiMessages.Payment.PaySuccess,
                        Data = new ProcessPaymentResult
                        {
                            Success = true,
                            Message = ApiMessages.Payment.PaySuccess,
                            TransactionCode = payment.TransactionCode,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount
                        }
                    };
                }

                if (payment.Status == PaymentStatus.Failed)
                {
                    _logger.LogInformation("PayOS payment {PaymentId} already processed as failed", payment.Id);
                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = true,
                        Message = ApiMessages.Payment.PaySuccess,
                        Data = new ProcessPaymentResult
                        {
                            Success = false,
                            Message = ApiMessages.Payment.PayFailed,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount
                        }
                    };
                }

                // 3. Determine success based on callback status using constants
                var isSuccess = PaymentResponseCode.PayOS.IsSuccess(status);
                var isCancelled = PaymentResponseCode.PayOS.IsCancelled(status);

                try
                {
                    // 4. Update payment status
                    if (isSuccess)
                    {
                        payment.Status = PaymentStatus.Success;
                        payment.PaidAt = DateTime.UtcNow;
                    }
                    else if (isCancelled)
                    {
                        payment.Status = PaymentStatus.Failed;
                        payment.ResponseMessage = PaymentResponseCode.PayOS.GetMessage(status);
                    }
                    // If PENDING or PROCESSING, don't change status yet

                    payment.PaidAt = DateTime.UtcNow;
                    await _unitOfWork.Payments.UpdateAsync(payment);

                    // 5. Update order and stock
                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);
                    if (order != null)
                    {
                        if (isSuccess)
                        {
                            order.Status = OrderStatus.Confirmed;
                            await _unitOfWork.Orders.UpdateAsync(order);
                            var confirmResult = await _stockReservationService.ConfirmStockReservationAsync(order.Id);
                            _logger.LogInformation(
                                "Order {OrderId} confirmed with PaymentMethod=PayOS, stock reservation confirmed. StockConfirmResult: {StockConfirmResult}",
                                order.Id, confirmResult.IsSuccess);
                        }
                        else if (isCancelled)
                        {
                            order.Status = OrderStatus.Cancelled;
                            await _unitOfWork.Orders.UpdateAsync(order);
                            var revertResult = await _stockReservationService.RevertStockReservationAsync(order.Id);
                            _logger.LogInformation(
                                "Order {OrderId} cancelled, stock reverted. StockRevertResult: {StockRevertResult}",
                                order.Id, revertResult.IsSuccess);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();

                    _logger.LogInformation(
                        "PayOS callback processed: PaymentId={PaymentId}, Status={Status}",
                        payment.Id, payment.Status);

                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = isSuccess,
                        Message = isSuccess ? ApiMessages.Payment.PaySuccess : ApiMessages.Payment.PayFailed,
                        Data = new ProcessPaymentResult
                        {
                            Success = isSuccess,
                            Message = isSuccess ? ApiMessages.Payment.PaySuccess : ApiMessages.Payment.PayFailed,
                            TransactionCode = payment.TransactionCode,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount
                        }
                    };
                }
                catch (Exception ex)
                {
                    //await _unitOfWork.RollbackAsync();
                    _logger.LogError(ex, "Error in PayOS callback transaction");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS callback");
                return new ServiceResult<ProcessPaymentResult>
                {
                    IsSuccess = false,
                    Message = ApiMessages.ServerError.General + $"\nError: {ex.Message}"
                };

            }
        }

        public async Task<ServiceResult<ProcessPaymentResult>> ProcessWebhookAsync(Webhook webhookBody)
        {
            try
            {
                _logger.LogInformation("ProcessWebhook: Attempting to verify signature");
                _logger.LogInformation("Webhook data: OrderCode={OrderCode}, Amount={Amount}", 
                    webhookBody?.Data?.OrderCode, webhookBody?.Data?.Amount);

                // 1. Verify webhook signature
                WebhookData verifiedData = await _payOS.Webhooks.VerifyAsync(webhookBody);

                var orderCode = verifiedData.OrderCode.ToString();

                _logger.LogInformation("Webhook signature verified successfully. OrderCode={OrderCode}", orderCode);

                // 2. Find payment by orderCode (stored as TransactionRef)
                var payment = await _unitOfWork.Payments.GetByOrderCodeAsync(orderCode, PaymentGateway.PayOS);

                if (payment == null)
                {
                    _logger.LogWarning("PayOS payment not found for OrderCode: {OrderCode}", orderCode);
                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = false,
                        Message = ApiMessages.Payment.NotFound
                    };
                }

                // 3. Prevent double processing
                if (payment.Status == PaymentStatus.Success)
                {
                    _logger.LogInformation("PayOS payment {PaymentId} already processed", payment.Id);
                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = true,
                        Message = ApiMessages.Payment.PaySuccess,
                        Data = new ProcessPaymentResult
                        {
                            Success = true,
                            Message = ApiMessages.Payment.PaySuccess,
                            OrderId = payment.OrderId,
                            Amount = payment.Amount
                        }
                    };
                }

                // 4. Determine success based on webhook code using constants
                var isSuccess = verifiedData.Code == PaymentResponseCode.PayOS.Success;

                //await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // 5. Update payment
                    payment.Status = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
                    payment.TransactionCode = verifiedData.Reference;
                    payment.ResponseCode = verifiedData.Code;
                    payment.ResponseMessage = verifiedData.Description;
                    payment.PaidAt = isSuccess ? DateTime.UtcNow : null;
                    payment.UpdateTime = DateTime.UtcNow;

                    await _unitOfWork.Payments.UpdateAsync(payment);

                    // 6. Update order status
                    var order = await _unitOfWork.Orders.GetByIdAsync(payment.OrderId);
                    if (order != null)
                    {
                        if (isSuccess)
                        {
                            order.Status = OrderStatus.Confirmed;
                            await _unitOfWork.Orders.UpdateAsync(order);
                            var confirmResult = await _stockReservationService.ConfirmStockReservationAsync(order.Id);
                            _logger.LogInformation(
                                "Order {OrderId} confirmed, stock reservation confirmed. StockConfirmResult: {StockConfirmResult}",
                                order.Id, confirmResult.IsSuccess);
                        }
                        else
                        {
                            order.Status = OrderStatus.Cancelled;
                            await _unitOfWork.Orders.UpdateAsync(order);
                            var revertResult = await _stockReservationService.RevertStockReservationAsync(order.Id);
                            _logger.LogInformation(
                                "Order {OrderId} cancelled, stock reverted. StockRevertResult: {StockRevertResult}",
                                order.Id, revertResult.IsSuccess);
                        }
                    }

                    await _unitOfWork.SaveChangesAsync();
                    //await _unitOfWork.CommitAsync();

                    _logger.LogInformation(
                        "PayOS payment {PaymentId} processed: Status={Status}, Amount={Amount}",
                        payment.Id, payment.Status, payment.Amount);

                    return new ServiceResult<ProcessPaymentResult>
                    {
                        IsSuccess = isSuccess,
                        Data = new ProcessPaymentResult
                        {
                            Success = isSuccess,
                            Message = isSuccess ? ApiMessages.Payment.PaySuccess : ApiMessages.Payment.PayFailed,
                            OrderId = payment.OrderId,
                            TransactionCode = verifiedData.Reference,
                            Amount = verifiedData.Amount
                        }
                    };

                }
                catch (Exception ex)
                {
                    //await _unitOfWork.RollbackAsync();
                    _logger.LogError(ex, "Error in PayOS payment transaction");
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing PayOS webhook - {ExceptionType}: {Message}", 
                    ex.GetType().Name, ex.Message);
                
                if (ex.Message.Contains("signature") || ex.Message.Contains("Kiểm tra"))
                {
                    _logger.LogError("SIGNATURE VERIFICATION FAILED! Please check:");
                    _logger.LogError("1. ChecksumKey in appsettings.json matches PayOS Dashboard");
                    _logger.LogError("2. Request body is being sent correctly");
                    _logger.LogError("3. Webhook URL is registered correctly in PayOS Dashboard");
                }
                
                return new ServiceResult<ProcessPaymentResult>
                {
                    IsSuccess = false,
                    Message = ApiMessages.ServerError.General + $"\nError: {ex.Message}"
                };
            }
        }


    }
}

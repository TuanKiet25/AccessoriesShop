using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using PayOS.Models.Webhooks;

namespace AccessoriesShop.Application.IServices
{
    public interface IPayOSService
    {
        /// <summary>
        /// Tạo link thanh toán PayOS
        /// </summary>
        Task<ServiceResult<PayOSResponse>> CreatePaymentLinkAsync(PayOSRequest request);

        /// <summary>
        /// Xử lý callback từ PayOS (redirect URL)
        /// Gọi khi user được redirect về sau khi thanh toán
        /// </summary>
        Task<ServiceResult<ProcessPaymentResult>> ProcessCallbackAsync(string orderCode, string status);

        /// <summary>
        /// Xử lý webhook từ PayOS (server-to-server)
        /// </summary>
        Task<ServiceResult<ProcessPaymentResult>> ProcessWebhookAsync(Webhook webhookBody);
    }
}
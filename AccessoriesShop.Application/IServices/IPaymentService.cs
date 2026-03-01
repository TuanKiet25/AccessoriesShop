using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    public interface IPaymentService
    {
        Task<ServiceResult<PaymentResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<PaymentResponse>>> GetAllAsync();
        Task<ServiceResult<List<PaymentResponse>>> GetByOrderIdAsync(Guid orderId);
        Task<ServiceResult<PaymentResponse>> CreateAsync(CreatePaymentRequest request);
        Task<ServiceResult<PaymentResponse>> UpdateAsync(Guid id, CreatePaymentRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

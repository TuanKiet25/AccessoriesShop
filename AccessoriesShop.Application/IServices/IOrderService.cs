using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    public interface IOrderService
    {
        Task<ServiceResult<OrderResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<OrderResponse>>> GetAllAsync();
        Task<ServiceResult<OrderResponse>> CreateAsync(CreateOrderRequest request);
        Task<ServiceResult<OrderResponse>> UpdateAsync(Guid id, CreateOrderRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

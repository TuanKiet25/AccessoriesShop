using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Interfaces.Services
{
    public interface ICartItemService
    {
        Task<ServiceResult<CartResponse>> CreateCardAsync();
        Task<ServiceResult<CartItemResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<CartItemResponse>>> GetAllAsync();
        Task<ServiceResult<List<CartItemResponse>>> GetAllByUserAsync();
        Task<ServiceResult<CartItemResponse>> CreateAsync(CartItemRequest request);
        Task<ServiceResult<CartItemResponse>> UpdateAsync(Guid id, CartItemRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

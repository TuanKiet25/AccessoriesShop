using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    public interface IAccountService
    {
        Task<ServiceResult<AccountResponse>> GetByIdAsync(Guid id);
        Task<ServiceResult<List<AccountResponse>>> GetAllAsync();
        Task<ServiceResult<AccountResponse>> UpdateAsync(Guid id, UpdateAccountRequest request);
        Task<ServiceResult<string>> DeleteAsync(Guid id);
    }
}

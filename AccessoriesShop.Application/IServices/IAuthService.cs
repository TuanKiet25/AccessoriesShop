using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.IServices
{
    public interface IAuthService
    {
        public Task<ServiceResult<string>> RegisterAsync(RegisterRequest request);
        public Task<ServiceResult<string>> LoginAsync(LoginRequest request);
        public Task<ServiceResult<string>> VerifyOtpAsync(VerifyOtpRequest request);
        public Task<ServiceResult<string>> ResendOtpAsync(ResendOtpRequest request);
    }
}

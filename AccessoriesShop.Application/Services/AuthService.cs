using AccessoriesShop.Application.IAuthentication;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;
        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
        }
        public async Task<ServiceResult<string>> LoginAsync(LoginRequest request)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetAsync(a => a.Email == request.Email);
                if (account is null || !_passwordHasher.Verify(request.Password, account.PasswordHash))
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid email or password."
                    };
                }
                return new ServiceResult<string> { Data =$"Login success with ID :{account.Id}, jwt Key:{_jwtProvider.Generate(account)}" , IsSuccess = true };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<string>> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var existingUser = await _unitOfWork.Accounts.GetAsync(a => a.Email == request.Email);
                if (existingUser != null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Email is already registered."
                    };
                }
                var account = _mapper.Map<Account>(request);
                account.PasswordHash = _passwordHasher.Hash(request.PasswordHash);
                account.Role = Domain.Enums.Role.User;
                await _unitOfWork.Accounts.AddAsync(account);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string> {  IsSuccess = true, Message = "Register successfully!" };
            }
            catch (Exception ex)
            {
                return new ServiceResult<string>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

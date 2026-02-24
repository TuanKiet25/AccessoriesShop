using AccessoriesShop.Application.IAuthentication;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using System;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtProvider _jwtProvider;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public AuthService(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, IJwtProvider jwtProvider, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _jwtProvider = jwtProvider;
            _mapper = mapper;
            _emailService = emailService;
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
                if (!account.IsActive)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Account is not verified. Please verify your OTP sent to your email."
                    };
                }
                return new ServiceResult<string> { Data = $"Login success with ID :{account.Id}, jwt Key:{_jwtProvider.Generate(account)}", IsSuccess = true };
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
                account.IsActive = false;
                await _unitOfWork.Accounts.AddAsync(account);

                // Tạo OTP và gửi mail
                var otpCode = GenerateOtpCode();
                var otp = new OtpVerification
                {
                    AccountId = account.Id,
                    OtpCode = otpCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(20),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.OtpVerifications.AddAsync(otp);
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendOtpEmailAsync(account.Email, account.Username, otpCode);

                return new ServiceResult<string> { IsSuccess = true, Message = "Register successfully! Please check your email for the OTP to verify your account." };
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

        public async Task<ServiceResult<string>> VerifyOtpAsync(VerifyOtpRequest request)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetAsync(a => a.Email == request.Email);
                if (account is null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Account not found."
                    };
                }

                if (account.IsActive)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Account is already verified."
                    };
                }

                var otps = await _unitOfWork.OtpVerifications.GetAllAsync(
                    o => o.AccountId == account.Id && !o.IsUsed);

                // Lấy OTP mới nhất chưa dùng
                OtpVerification? validOtp = null;
                foreach (var o in otps)
                {
                    if (o.OtpCode == request.OtpCode && o.ExpiresAt > DateTime.UtcNow)
                    {
                        validOtp = o;
                        break;
                    }
                }

                if (validOtp is null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Invalid or expired OTP code."
                    };
                }

                // Đánh dấu OTP đã dùng
                validOtp.IsUsed = true;
                await _unitOfWork.OtpVerifications.UpdateAsync(validOtp);

                // Kích hoạt tài khoản
                account.IsActive = true;
                account.UpdateTime = DateTime.UtcNow;
                await _unitOfWork.Accounts.UpdateAsync(account);

                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<string> { IsSuccess = true, Message = "Account verified successfully! You can now log in." };
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

        public async Task<ServiceResult<string>> ResendOtpAsync(ResendOtpRequest request)
        {
            try
            {
                var account = await _unitOfWork.Accounts.GetAsync(a => a.Email == request.Email);
                if (account is null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Account not found."
                    };
                }

                if (account.IsActive)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        Message = "Account is already verified."
                    };
                }

                // Tạo OTP mới
                var otpCode = GenerateOtpCode();
                var otp = new OtpVerification
                {
                    AccountId = account.Id,
                    OtpCode = otpCode,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(20),
                    IsUsed = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.OtpVerifications.AddAsync(otp);
                await _unitOfWork.SaveChangesAsync();

                await _emailService.SendOtpEmailAsync(account.Email, account.Username, otpCode);

                return new ServiceResult<string> { IsSuccess = true, Message = "OTP has been resent to your email." };
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

        private static string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }
    }
}

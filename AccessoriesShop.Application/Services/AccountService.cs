using AccessoriesShop.Application.Interfaces;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AccountService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ServiceResult<AccountResponse>> GetByIdAsync(Guid id)
        {
            try
            {
                var entity = await _unitOfWork.Accounts.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<AccountResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Account not found."
                    };
                }
                return new ServiceResult<AccountResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AccountResponse>(entity)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AccountResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<List<AccountResponse>>> GetAllAsync()
        {
            try
            {
                var entities = await _unitOfWork.Accounts.GetAllAsync(null);
                return new ServiceResult<List<AccountResponse>>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<List<AccountResponse>>(entities)
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<List<AccountResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceResult<AccountResponse>> UpdateAsync(Guid id, UpdateAccountRequest request)
        {
            try
            {
                var entity = await _unitOfWork.Accounts.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<AccountResponse>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Account not found."
                    };
                }

                // Check if email already exists (if trying to update email)
                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != entity.Email)
                {
                    var existingAccount = await _unitOfWork.Accounts
                        .GetAsync(a => a.Email == request.Email && a.IsActive == true);
                    if (existingAccount != null)
                    {
                        return new ServiceResult<AccountResponse>
                        {
                            IsSuccess = false,
                            Message = "Email is already in use."
                        };
                    }
                }

                _mapper.Map(request, entity);
                await _unitOfWork.Accounts.UpdateAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                return new ServiceResult<AccountResponse>
                {
                    IsSuccess = true,
                    Data = _mapper.Map<AccountResponse>(entity),
                    Message = "Account updated successfully."
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<AccountResponse>
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
                var entity = await _unitOfWork.Accounts.GetByIdAsync(id);
                if (entity == null)
                {
                    return new ServiceResult<string>
                    {
                        IsSuccess = false,
                        IsNotFound = true,
                        Message = "Account not found."
                    };
                }
                await _unitOfWork.Accounts.DeleteAsync(id);
                await _unitOfWork.SaveChangesAsync();
                return new ServiceResult<string>
                {
                    IsSuccess = true,
                    Message = "Account deleted successfully."
                };
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

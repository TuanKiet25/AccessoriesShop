using AccessoriesShop.Application.DTOs.ChatboxDto;
using AccessoriesShop.Application.Interfaces.External;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Services
{
    public class ChatboxService : IChatboxService
    {
        private readonly IProductRepository _productRepository;
        private readonly IAIIntegrationService _aiService;
        private readonly IChatRoomService _chatRoomService;
        private readonly ILogger<ChatboxService> _logger;

        public ChatboxService(
              IAIIntegrationService aiService,
              IProductRepository productRepository,
              IChatRoomService chatRoomService,
              ILogger<ChatboxService> logger)
        {
            _aiService = aiService;
            _productRepository = productRepository;
            _chatRoomService = chatRoomService;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ChatRoomResponse>>> GetAllChatRoomAsync()
        {
            try
            {
                var rooms = await _chatRoomService.GetActiveRoomsAsync();
                return new ServiceResult<IEnumerable<ChatRoomResponse>>
                {
                    IsSuccess = true,
                    Data = rooms,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching chat rooms");
                return new ServiceResult<IEnumerable<ChatRoomResponse>>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<ServiceStatusDto> GetServiceStatusAsync()
        {
            return new ServiceStatusDto
            {
                IsAvailable = IsAvailable(),
                //Provider = _aiService ,
                //Model = _aiService ,
                Message = IsAvailable()
                    ? "Dich vu chatbox AI dang hoat dong binh thuong"
                    : "Dich vu chatbox AI tam thoi khong kha dung"
            };
        }


        public bool IsAvailable() => true;

        public async Task<ServiceResult<ChatboxResponse>> SendMessageAsync(ChatboxRequest request, Guid? userId = null)
        {
            try
            {
                var aiResult = await _aiService.GetChatResponseAsync(request.Message, null, request.Language);

                if (!aiResult.IsSuccess)
                {
                    return new ServiceResult<ChatboxResponse>
                    {
                        IsSuccess = false,
                        Message = aiResult.ErrorMessage ?? "AI service failed"
                    };
                }

                var response = new ChatboxResponse
                {
                    IsSuccessful = true,
                    Message = aiResult.Response,
                    ResponseType = "general",
                    RespondedAt = DateTime.UtcNow,
                    DebugInfo = new ChatboxDebugInfo
                    {
                        Provider = aiResult.DebugInfo?.Provider,
                        Model = aiResult.DebugInfo?.Model,
                        ResponseTimeMs = aiResult.DebugInfo?.ResponseTimeMs ?? 0,
                        HasImage = false,
                        HttpStatusCode = aiResult.DebugInfo?.HttpStatusCode
                    }
                };

                return new ServiceResult<ChatboxResponse>
                {
                    IsSuccess = true,
                    Data = response,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatboxService.SendMessageAsync");
                return new ServiceResult<ChatboxResponse>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}

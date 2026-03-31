using AccessoriesShop.Application.DTOs.ChatboxDto;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Application.ViewModels.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Interfaces.Services
{
    public interface IChatboxService
    {
        /// <summary>
        /// Check if chatbox AI service is available
        /// </summary>
        /// <returns>True if service is ready</returns>
        bool IsAvailable();

        /// <summary>
        /// Send a message to the chatbox AI and get a response
        /// Supports text-only and image+text conversations
        /// </summary>
        /// <param name="request">Chatbox request with user message and optional image</param>
        /// <param name="userId">Optional user ID for conversation history tracking</param>
        /// <returns>AI response with message, recommendations, and optional diagnosis</returns>
        Task<ServiceResult<ChatboxResponse>> SendMessageAsync(ChatboxRequest request, Guid? userId = null);

        /// <summary>
        /// Get service status and model information
        /// </summary>
        /// <returns>Service status details</returns>
        Task<ServiceStatusDto> GetServiceStatusAsync();
    }

}

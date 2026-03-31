using AccessoriesShop.Application.DTOs.ChatboxDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.Interfaces.External
{
    public interface IAIIntegrationService
    {
        Task<AIChatResult> GetChatResponseAsync(string message, byte[]? image = null, string language = "vi");

    }
}

using AccessoriesShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.DTOs.ChatboxDto
{
    public class AIRequest
    {
        public string? SystemPrompt { get; set; } = string.Empty;
        public string Prompt { get; set; }
        public byte[]? ImageData { get; set; }
        public AICapability RequiredCapability { get; set; }
    }
}

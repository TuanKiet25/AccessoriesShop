using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.DTOs.ChatboxDto
{
    public class AIChatResult
    {

    public bool IsSuccess { get; set; }

        /// <summary>
        /// Conversational response from AI
        /// </summary>
        public string? Response { get; set; }

        /// <summary>
        /// Error message if failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Debug info
        /// </summary>
        public ChatDebugInfo DebugInfo { get; set; } = new();
    }

    public class ChatDebugInfo
    {
        public string? Provider { get; set; }
        public string? Model { get; set; }
        public int? HttpStatusCode { get; set; }
        public long ResponseTimeMs { get; set; }
    }
}

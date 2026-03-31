using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.DTOs.ChatboxDto
{

    /// <summary>
    /// Service status information
    /// </summary>
    public class ServiceStatusDto
    {
        /// <summary>
        /// Whether service is available
        /// </summary>
        public bool IsAvailable { get; set; }

        /// <summary>
        /// AI provider name (Groq, Gemini, etc.)
        /// </summary>
        public string? Provider { get; set; }

        /// <summary>
        /// AI model name
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Status message
        /// </summary>
        public string? Message { get; set; }
    }
}

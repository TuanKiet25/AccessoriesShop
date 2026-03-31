using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ChatboxResponse
    {
        /// <summary>
        /// Whether the response was successful
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// Error message if failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// AI response message to user
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Type of response (general, diagnosis, recommendation, etc.)
        /// </summary>
        public string ResponseType { get; set; } = "general"; // general, diagnosis, care_advice, product_recommendation


        /// <summary>
        /// Additional context or follow-up suggestions
        /// </summary>
        public List<string>? SuggestedFollowUps { get; set; }

        /// <summary>
        /// Debug info (only in development environment)
        /// </summary>
        public ChatboxDebugInfo? DebugInfo { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// mốt dọn lại sau
    /// </summary>
    public class ChatboxDebugInfo
    {
        /// <summary>
        /// AI provider used (Groq, Gemini, etc.)
        /// </summary>
        public string? Provider { get; set; }

        /// <summary>
        /// AI model name
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Whether image was analyzed
        /// </summary>
        public bool HasImage { get; set; }

        /// <summary>
        /// Response time in milliseconds
        /// </summary>
        public long ResponseTimeMs { get; set; }

        /// <summary>
        /// Cache hit indicator
        /// </summary>
        public bool CacheHit { get; set; }

        /// <summary>
        /// Any error from AI provider
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// HTTP status code from external service
        /// </summary>
        public int? HttpStatusCode { get; set; }
    }

}

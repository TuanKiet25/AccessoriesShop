using AccessoriesShop.Application.DTOs.ChatboxDto;
using AccessoriesShop.Application.Interfaces.External;
using AccessoriesShop.Domain.Enums;
using System.Diagnostics;

namespace AccessoriesShop.Application.Services.AIServices
{
    public class AIIntegrationService : IAIIntegrationService
    {
        private readonly IEnumerable<IAIProvider> _providers;

        public AIIntegrationService(IEnumerable<IAIProvider> providers)
        {
            _providers = providers;
        }

        public async Task<AIChatResult> GetChatResponseAsync(string message, byte[]? image = null, string language = "vi")
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new AIChatResult();

            try
            {
                var cap = image != null ? AICapability.ImageVision : AICapability.TextGeneration;

                var provider = _providers.FirstOrDefault(p => p.SkillSupports(cap) && p.IsAvailableAsync().Result);

                if (provider == null) throw new Exception("No AI provider available for this task.");

                var aiResponse = await provider.ExecuteAsync(new AIRequest { Prompt = message, ImageData = image });

                result.IsSuccess = true;
                result.Response = aiResponse;
                result.DebugInfo = new ChatDebugInfo
                {
                    Provider = provider.ProviderName,  
                    Model = provider.ModelName,  
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    HttpStatusCode = 200
                };
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = ex.Message;
                result.DebugInfo.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            }

            return result;
        }


    }
}

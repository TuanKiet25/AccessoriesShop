using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.Interfaces.Services
{
    public interface IAIVisionService
    {
        bool IsAvailable();
        string GetModelName();
        string GetProviderName();

        Task<AIAnalysisResult> AnalyzePlantImageAsync(
            string? imageBase64,
            string? imageUrl,
            string? userDescription,
            string language = "vi");
    }
}

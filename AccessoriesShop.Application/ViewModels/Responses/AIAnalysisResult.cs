namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class AIAnalysisResult
    {
        public bool Success { get; set; }
        public string? Description { get; set; }
        public string? SuggestedCategory { get; set; }
        public string? SuggestedBrand { get; set; }
        public List<string> Colors { get; set; } = new();
        public List<string> SuggestedAttributes { get; set; } = new();
        public string? RawResponse { get; set; }
        public string? ErrorMessage { get; set; }
    }
}

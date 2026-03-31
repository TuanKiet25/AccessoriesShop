namespace AccessoriesShop.Application.Common.Settings
{
    public class GroqSettings
    {
        public const string SectionName = "Groq";
        public string ApiKey { get; set; } = string.Empty;
        public string ModelName { get; set; } = "llama-3.2-11b-vision-preview";
        public string BaseUrl { get; set; } = "https://api.groq.com/openai/v1/chat/completions";
    }
}

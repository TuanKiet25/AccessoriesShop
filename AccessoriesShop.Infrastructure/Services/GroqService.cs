using AccessoriesShop.Application.Common.Settings;
using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Responses;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AccessoriesShop.Infrastructure.Services
{
    public class GroqVisionService : IAIVisionService
    {
        private readonly GroqSettings _settings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<GroqVisionService> _logger;

        public GroqVisionService(
            IOptions<GroqSettings> settings,
            IHttpClientFactory httpClientFactory,
            ILogger<GroqVisionService> logger)
        {
            _settings = settings.Value;
            _httpClient = httpClientFactory.CreateClient("Groq");
            _logger = logger;
        }

        public bool IsAvailable() => !string.IsNullOrWhiteSpace(_settings.ApiKey);

        public string GetModelName() => _settings.ModelName;

        public string GetProviderName() => "Groq";

        public async Task<AIAnalysisResult> AnalyzePlantImageAsync(
            string? imageBase64,
            string? imageUrl,
            string? userDescription,
            string language = "vi")
        {
            if (!IsAvailable())
                return new AIAnalysisResult { Success = false, ErrorMessage = "Groq API key is not configured." };

            if (string.IsNullOrWhiteSpace(imageBase64) && string.IsNullOrWhiteSpace(imageUrl))
                return new AIAnalysisResult { Success = false, ErrorMessage = "Either imageBase64 or imageUrl must be provided." };

            try
            {
                var systemPrompt = language == "vi"
                    ? "Bạn là trợ lý phân tích sản phẩm phụ kiện điện thoại. Hãy phân tích hình ảnh và trả về thông tin dưới dạng JSON."
                    : "You are a phone accessories product analyst. Analyze the image and return information in JSON format.";

                var userPrompt = language == "vi"
                    ? BuildViPrompt(userDescription)
                    : BuildEnPrompt(userDescription);

                var imageContent = BuildImageContent(imageBase64, imageUrl);

                var requestBody = new
                {
                    model = _settings.ModelName,
                    messages = new object[]
                    {
                        new { role = "system", content = systemPrompt },
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new { type = "text", text = userPrompt },
                                imageContent
                            }
                        }
                    },
                    max_tokens = 1024,
                    temperature = 0.2
                };

                var json = JsonSerializer.Serialize(requestBody);
                var request = new HttpRequestMessage(HttpMethod.Post, _settings.BaseUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);

                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Groq API error: {StatusCode} - {Body}", response.StatusCode, responseBody);
                    return new AIAnalysisResult { Success = false, ErrorMessage = $"Groq API returned {response.StatusCode}." };
                }

                return ParseGroqResponse(responseBody);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling Groq vision API");
                return new AIAnalysisResult { Success = false, ErrorMessage = ex.Message };
            }
        }

        private static object BuildImageContent(string? imageBase64, string? imageUrl)
        {
            if (!string.IsNullOrWhiteSpace(imageBase64))
            {
                var mimeType = imageBase64.StartsWith("/9j/") ? "image/jpeg" : "image/png";
                return new
                {
                    type = "image_url",
                    image_url = new { url = $"data:{mimeType};base64,{imageBase64}" }
                };
            }

            return new
            {
                type = "image_url",
                image_url = new { url = imageUrl }
            };
        }

        private static string BuildViPrompt(string? userDescription)
        {
            var desc = string.IsNullOrWhiteSpace(userDescription)
                ? ""
                : $"\nMô tả thêm từ người dùng: {userDescription}";

            return $@"Phân tích hình ảnh sản phẩm phụ kiện điện thoại này.{desc}

Trả về JSON với cấu trúc:
{{
  ""description"": ""mô tả ngắn sản phẩm"",
  ""suggestedCategory"": ""danh mục đề xuất (ví dụ: Ốp lưng, Tai nghe, Cáp sạc, Kính cường lực)"",
  ""suggestedBrand"": ""thương hiệu nếu nhận ra, nếu không thì null"",
  ""colors"": [""danh sách màu sắc""],
  ""suggestedAttributes"": [""các thuộc tính khác như chất liệu, loại kết nối, v.v.""]
}}";
        }

        private static string BuildEnPrompt(string? userDescription)
        {
            var desc = string.IsNullOrWhiteSpace(userDescription)
                ? ""
                : $"\nAdditional description from user: {userDescription}";

            return $@"Analyze this phone accessory product image.{desc}

Return JSON with structure:
{{
  ""description"": ""short product description"",
  ""suggestedCategory"": ""suggested category (e.g. Phone Case, Earphones, Charging Cable, Screen Protector)"",
  ""suggestedBrand"": ""brand if recognizable, otherwise null"",
  ""colors"": [""list of colors""],
  ""suggestedAttributes"": [""other attributes like material, connection type, etc.""]
}}";
        }

        private AIAnalysisResult ParseGroqResponse(string responseBody)
        {
            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                var content = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content")
                    .GetString() ?? string.Empty;

                // Extract JSON from the response content
                var jsonStart = content.IndexOf('{');
                var jsonEnd = content.LastIndexOf('}');
                if (jsonStart < 0 || jsonEnd < 0)
                    return new AIAnalysisResult { Success = true, RawResponse = content };

                var jsonContent = content[jsonStart..(jsonEnd + 1)];
                using var parsed = JsonDocument.Parse(jsonContent);
                var root = parsed.RootElement;

                var result = new AIAnalysisResult
                {
                    Success = true,
                    RawResponse = content,
                    Description = GetStringProp(root, "description"),
                    SuggestedCategory = GetStringProp(root, "suggestedCategory"),
                    SuggestedBrand = GetStringProp(root, "suggestedBrand"),
                };

                if (root.TryGetProperty("colors", out var colors) && colors.ValueKind == JsonValueKind.Array)
                    result.Colors = colors.EnumerateArray().Select(c => c.GetString() ?? "").Where(c => c != "").ToList();

                if (root.TryGetProperty("suggestedAttributes", out var attrs) && attrs.ValueKind == JsonValueKind.Array)
                    result.SuggestedAttributes = attrs.EnumerateArray().Select(a => a.GetString() ?? "").Where(a => a != "").ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing Groq response: {Body}", responseBody);
                return new AIAnalysisResult { Success = false, ErrorMessage = "Failed to parse AI response.", RawResponse = responseBody };
            }
        }

        private static string? GetStringProp(JsonElement element, string key)
        {
            return element.TryGetProperty(key, out var prop) && prop.ValueKind == JsonValueKind.String
                ? prop.GetString()
                : null;
        }
    }
}

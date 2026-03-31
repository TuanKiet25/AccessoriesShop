using AccessoriesShop.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/ai-vision")]
    [ApiController]
    public class AIVisionController : MyBaseController
    {
        private readonly IAIVisionService _aiVisionService;

        public AIVisionController(IAIVisionService aiVisionService)
        {
            _aiVisionService = aiVisionService;
        }

        /// <summary>
        /// Phân tích hình ảnh sản phẩm bằng AI
        /// </summary>
        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeImage([FromBody] AnalyzeImageRequest request)
        {
            if (!_aiVisionService.IsAvailable())
                return StatusCode(503, "AI Vision service is not configured.");

            var result = await _aiVisionService.AnalyzePlantImageAsync(
                request.ImageBase64,
                request.ImageUrl,
                request.UserDescription,
                request.Language ?? "vi");

            if (!result.Success)
                return BadRequest(result.ErrorMessage);

            return Ok(result);
        }

        /// <summary>
        /// Kiểm tra trạng thái AI Vision service
        /// </summary>
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new
            {
                available = _aiVisionService.IsAvailable(),
                provider = _aiVisionService.GetProviderName(),
                model = _aiVisionService.GetModelName()
            });
        }
    }

    public class AnalyzeImageRequest
    {
        public string? ImageBase64 { get; set; }
        public string? ImageUrl { get; set; }
        public string? UserDescription { get; set; }
        public string? Language { get; set; } = "vi";
    }
}

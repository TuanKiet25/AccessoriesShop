using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/custom-order")]
    [ApiController]
    public class CustomOrderController : MyBaseController
    {
        private readonly ICustomOrderService _customOrderService;

        public CustomOrderController(ICustomOrderService customOrderService)
        {
            _customOrderService = customOrderService;
        }

        /// <summary>
        /// Tạo yêu cầu đặt hàng tùy chỉnh
        /// </summary>
        /// <remarks>
        /// API dùng để tạo custom order cho khách hàng, hỗ trợ cả guest và user đăng nhập.
        /// </remarks>
        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCustomOrderRequest request)
        {
            var response = await _customOrderService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPost("{id:guid}/quote")]
        public async Task<IActionResult> Quote(Guid id, [FromBody] QuoteCustomOrderRequest request)
        {
            var response = await _customOrderService.QuoteAsync(id, request);
            return HandleResult(response);
        }

        [HttpPost("{id:guid}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateCustomOrderStatusRequest request)
        {
            var response = await _customOrderService.UpdateStatusAsync(id, request);
            return HandleResult(response);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _customOrderService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _customOrderService.GetAllAsync();
            return HandleResult(response);
        }
        
        
        
    }
}

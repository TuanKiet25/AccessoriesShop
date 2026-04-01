using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : MyBaseController
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _orderService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _orderService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("get-my")]
        public async Task<IActionResult> GetMy([FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("User not authenticated.");
            }
            request.AccountId = userId;
            var response = await _orderService.CreateAsync( request);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("User not authenticated.");
            }
            request.AccountId = userId;
            var response = await _orderService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrderRequest request)
        {
            var userId = GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("User not authenticated.");
            }
            request.AccountId = userId;
            var response = await _orderService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("User not authenticated.");
            }
            var response = await _orderService.DeleteAsync(id);
            return HandleResult(response);
        }
        [HttpPost("Create-order-by-card-Items-id")]
        public async Task<IActionResult> CreateOrderByCartId(List<Guid> cartItemIds, string? ReceiverName, string? ReceiverPhone, Guid AddressId)
        {
            var response = await _orderService.PlaceOrderFromSelectedItemsAsync(cartItemIds, ReceiverName, ReceiverPhone, AddressId);
            return HandleResult(response);
        }
        [HttpPut("update-shipping-detail/{orderId}")]
        public async Task<IActionResult> UpdateShippingDetail(Guid orderId, string? ReceiverName, string? ReceiverPhone, Guid AddressId)
        {
            var userId = GetUserId(User);
            if (userId == Guid.Empty)
            {
                return BadRequest("User not authenticated.");
            }
            var response = await _orderService.UpdateShippingDetailAsync(orderId, ReceiverName, ReceiverPhone, AddressId);
            return HandleResult(response);
        }
        [HttpGet("get-order-by-user-id")]
        public async Task<IActionResult> GetOrderByUserId()
        {
            var response = await _orderService.GetOrderByUserId();
            return HandleResult(response);
        }
        // Helper method to extract user id from ClaimsPrincipal
        private Guid GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
        
    }
}
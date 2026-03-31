using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/cart-item")]
    [ApiController]
    public class CartItemController : MyBaseController
    {
        private readonly ICartItemService _cartItemService;

        public CartItemController(ICartItemService cartItemService)
        {
            _cartItemService = cartItemService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _cartItemService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _cartItemService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CartItemRequest request)
        {
            var response = await _cartItemService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CartItemRequest request)
        {
            var response = await _cartItemService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _cartItemService.DeleteAsync(id);
            return HandleResult(response);
        }
        [HttpPost("create-cart")]
        public async Task<IActionResult> CreateCart()
        {
            var response = await _cartItemService.CreateCardAsync();
            return HandleResult(response);
        }
        [HttpGet("get-all-by-user")]
        public async Task<IActionResult> GetAllByUser()
        {
            var response = await _cartItemService.GetAllByUserAsync();
            return HandleResult(response);
        }
    }
}

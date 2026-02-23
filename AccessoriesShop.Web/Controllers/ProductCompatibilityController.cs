using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/product-compatibility")]
    [ApiController]
    public class ProductCompatibilityController : MyBaseController
    {
        private readonly IProductCompatibilityService _productCompatibilityService;

        public ProductCompatibilityController(IProductCompatibilityService productCompatibilityService)
        {
            _productCompatibilityService = productCompatibilityService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productCompatibilityService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productCompatibilityService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateProductCompatibilityRequest request)
        {
            var response = await _productCompatibilityService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductCompatibilityRequest request)
        {
            var response = await _productCompatibilityService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _productCompatibilityService.DeleteAsync(id);
            return HandleResult(response);
        }
    }
}

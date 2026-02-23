using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/product-variant")]
    [ApiController]
    public class ProductVariantController : MyBaseController
    {
        private readonly IProductVariantService _productVariantService;

        public ProductVariantController(IProductVariantService productVariantService)
        {
            _productVariantService = productVariantService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _productVariantService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _productVariantService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateProductVariantRequest request)
        {
            var response = await _productVariantService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateProductVariantRequest request)
        {
            var response = await _productVariantService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _productVariantService.DeleteAsync(id);
            return HandleResult(response);
        }
    }
}

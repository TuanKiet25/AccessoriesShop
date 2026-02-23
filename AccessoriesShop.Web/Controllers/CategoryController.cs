using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/category")]
    [ApiController]
    public class CategoryController : MyBaseController
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _categoryService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
        {
            var response = await _categoryService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateCategoryRequest request)
        {
            var response = await _categoryService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _categoryService.DeleteAsync(id);
            return HandleResult(response);
        }
    }
}

using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
	[Route("api/promotion")]
	[ApiController]
	public class PromotionController : MyBaseController
	{
		private readonly IPromotionService _promotionService;

		public PromotionController(IPromotionService promotionService)
		{
			_promotionService = promotionService;
		}

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			var response = await _promotionService.GetAllAsync();
			return HandleResult(response);
		}

		[HttpGet("get-by-id/{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _promotionService.GetByIdAsync(id);
			return HandleResult(response);
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
		{
			var response = await _promotionService.CreateAsync(request);
			return HandleResult(response);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePromotionRequest request)
		{
			var response = await _promotionService.UpdateAsync(id, request);
			return HandleResult(response);
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _promotionService.DeleteAsync(id);
			return HandleResult(response);
		}
	}
}

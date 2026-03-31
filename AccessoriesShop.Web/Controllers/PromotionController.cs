using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
	[Route("api/promotions")]
	public class PromotionController : MyBaseController
	{
		private readonly IPromotionService _promotionService;

		public PromotionController(IPromotionService promotionService)
		{
			_promotionService = promotionService;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] CreatePromotionRequest request)
		{
			await _promotionService.CreateAsync(request);
			return Ok(new { message = "Promotion created successfully" });
		}

		[HttpGet("product/{productId:guid}/active")]
		public async Task<IActionResult> GetActive(Guid productId)
		{
			var promotion = await _promotionService.GetActiveByProductIdAsync(productId);
			return Ok(promotion);
		}
	}
}

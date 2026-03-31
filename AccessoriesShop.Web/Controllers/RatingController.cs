using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccessoriesShop.Web.Controllers
{
	[Route("api/ratings")]
	public class RatingController : MyBaseController
	{
		private readonly IRatingService _ratingService;

		public RatingController(IRatingService ratingService)
		{
			_ratingService = ratingService;
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateOrUpdate([FromBody] CreateRatingRequest request)
		{
			var accountId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			await _ratingService.CreateOrUpdateAsync(accountId, request);
			return Ok(new { message = "Rating saved successfully" });
		}

		[HttpGet("product/{productId:guid}/summary")]
		public async Task<IActionResult> GetSummary(Guid productId)
		{
			var average = await _ratingService.GetAverageAsync(productId);
			var total = await _ratingService.GetTotalAsync(productId);
			return Ok(new { productId, averageRating = average, totalRatings = total });
		}
	}
}

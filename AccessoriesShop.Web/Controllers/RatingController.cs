using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AccessoriesShop.Web.Controllers
{
	[Route("api/rating")]
	[ApiController]
	public class RatingController : MyBaseController
	{
		private readonly IRatingService _ratingService;

		public RatingController(IRatingService ratingService)
		{
			_ratingService = ratingService;
		}

		[HttpGet("get-all")]
		public async Task<IActionResult> GetAll()
		{
			var response = await _ratingService.GetAllAsync();
			return HandleResult(response);
		}

		[HttpGet("get-by-id/{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var response = await _ratingService.GetByIdAsync(id);
			return HandleResult(response);
		}

		[HttpPost("create")]
		public async Task<IActionResult> Create([FromBody] CreateRatingRequest request)
		{
			var response = await _ratingService.CreateAsync(request);
			return HandleResult(response);
		}

		[HttpPut("update/{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRatingRequest request)
		{
			var response = await _ratingService.UpdateAsync(id, request);
			return HandleResult(response);
		}

		[HttpDelete("delete/{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var response = await _ratingService.DeleteAsync(id);
			return HandleResult(response);
		}
	}
}

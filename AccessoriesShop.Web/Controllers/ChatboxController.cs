using AccessoriesShop.Application.Interfaces.Services;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    public class ChatboxController : MyBaseController
    {
        private readonly IChatboxService _chatboxService;

        public ChatboxController(IChatboxService chatboxService)
        {
            _chatboxService = chatboxService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatboxRequest request)
        {
            var result = await _chatboxService.SendMessageAsync(request);
            return HandleResult(result);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            var status = await _chatboxService.GetServiceStatusAsync();
            return Ok(status);
        }
    }
}

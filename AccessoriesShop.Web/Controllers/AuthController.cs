using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : MyBaseController
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return HandleResult(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            return HandleResult(response);
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var response = await _authService.VerifyOtpAsync(request);
            return HandleResult(response);
        }

        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var response = await _authService.ResendOtpAsync(request);
            return HandleResult(response);
        }
    }
}

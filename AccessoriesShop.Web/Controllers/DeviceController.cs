using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/device")]
    [ApiController]
    public class DeviceController : MyBaseController
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _deviceService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _deviceService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateDeviceRequest request)
        {
            var response = await _deviceService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateDeviceRequest request)
        {
            var response = await _deviceService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _deviceService.DeleteAsync(id);
            return HandleResult(response);
        }
    }
}

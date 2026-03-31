using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AccessoriesShop.Web.Controllers
{
    [Route("api/address")]
    [ApiController]
    public class AddressController : MyBaseController
    {
        private readonly IAddressService _addressService;

        public AddressController(IAddressService addressService)
        {
            _addressService = addressService;
        }

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _addressService.GetAllAsync();
            return HandleResult(response);
        }

        [HttpGet("get-by-id/{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _addressService.GetByIdAsync(id);
            return HandleResult(response);
        }

        [HttpGet("get-by-account/{accountId}")]
        public async Task<IActionResult> GetByAccountId(Guid accountId)
        {
            var response = await _addressService.GetByAccountIdAsync(accountId);
            return HandleResult(response);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] CreateAddressRequest request)
        {
            var response = await _addressService.CreateAsync(request);
            return HandleResult(response);
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateAddressRequest request)
        {
            var response = await _addressService.UpdateAsync(id, request);
            return HandleResult(response);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _addressService.DeleteAsync(id);
            return HandleResult(response);
        }
    }
}

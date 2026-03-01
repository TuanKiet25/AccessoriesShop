using AccessoriesShop.Application.Common.Constants;
using AccessoriesShop.Application.Common.Settings;
using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayOS.Models.Webhooks;

namespace AccessoriesShop.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        private readonly IPaymentService _paymentService;
        private readonly ClientSettings _clientSettings;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            IPayOSService payOSService,
            IPaymentService paymentService,
            IOptions<ClientSettings> clientSettings,
            ILogger<PaymentsController> logger)
        {
            _clientSettings = clientSettings.Value;
            _paymentService = paymentService;
            _payOSService = payOSService;
            _logger = logger;
        }


        /// <summary>
        /// Create PayOS payment link
        /// </summary>
        /// <param name="request">Payment request data</param>
        /// <returns>PayOS payment URL</returns>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /api/payments/payos/create
        ///     {
        ///         "orderId": "guid",  // ID don hang can thanh toan
        ///     }
        /// </remarks>
        /// <response code="200">Payment link created successfully</response>
        /// <response code="400">Invalid data or order not found</response>
        /// <response code="401">Unauthorized</response>
        [HttpPost("payos/create")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreatePayOSPayment([FromBody] PayOSRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

           var result = await _payOSService.CreatePaymentLinkAsync(request);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// PayOS callback (Return URL) - Handles redirect from PayOS after payment
        /// </summary>
        /// <param name="callbackDto">PayOS callback parameters</param>
        /// <returns>Redirect to frontend success/failure page</returns>
        /// <response code="302">Redirect to frontend</response>
        [HttpGet("payos/callback")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status302Found)]
        public async Task<IActionResult> PayOSCallback([FromQuery] PayOSCallBackDto callbackDto)
        {
            _logger.LogInformation(
                "PayOS callback: code={Code}, orderCode={OrderCode}, status={Status}",
                callbackDto.code, callbackDto.orderCode, callbackDto.status);

            // Process the callback to update payment and order status
            var result = await _payOSService.ProcessCallbackAsync(callbackDto.orderCode, callbackDto.status);

            if (result.IsSuccess && callbackDto.status == PaymentResponseCode.Success)
            {
                var successUrl = $"{_clientSettings.BaseUrl}?orderCode={callbackDto.orderCode}&status=success";
                return Redirect(successUrl);
            }

            var failUrl = $"{_clientSettings.BackupUrl}?orderCode={callbackDto.orderCode}&status=failed&message={callbackDto.status}";
            return Redirect(failUrl);
        }

        /// <summary>
        /// PayOS webhook (IPN) - Server-to-server notification
        /// </summary>
        /// <param name="webhookBody">PayOS webhook data</param>
        /// <returns>Acknowledgement response</returns>
        /// <response code="200">Webhook processed</response>
        [HttpPost("payos/webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> PayOSWebhook([FromBody] Webhook webhookBody)
        {
            _logger.LogInformation("PayOS webhook received");

            try
            {
                var result = await _payOSService.ProcessWebhookAsync(webhookBody);

                return Ok(IpnResponse.FromResult(result.IsSuccess, result.Message ?? PaymentStatus.Success));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PayOS webhook error: {Message}", ex.Message);
                // Return 200 to acknowledge webhook receipt, but log the error
                return Ok(IpnResponse.FromResult(false, $"Error: {ex.Message}"));
            }
        }

        // ============================================
        // PAYMENT QUERIES
        // ============================================

        /// <summary>
        /// Get payment by ID
        /// </summary>
        /// <param name="id">Payment ID (GUID)</param>
        /// <returns>Payment data</returns>
        /// <response code="200">Payment found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Payment not found</response>
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _paymentService.GetByIdAsync(id);
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        /// <summary>
        /// Get all payments for an order
        /// </summary>
        /// <param name="orderId">Order ID (GUID)</param>
        /// <returns>List of payments</returns>
        /// <response code="200">List of payments</response>
        /// <response code="400">Error occurred</response>
        /// <response code="401">Unauthorized</response>
        [HttpGet("order/{orderId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetByOrderId(Guid orderId)
        {
            var result = await _paymentService.GetByOrderIdAsync(orderId);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

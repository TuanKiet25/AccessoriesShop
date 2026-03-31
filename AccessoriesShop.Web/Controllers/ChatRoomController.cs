using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.ViewModels.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccessoriesShop.Web.Controllers
{
    public class ChatRoomController : MyBaseController
    {
        private readonly IChatRoomService _chatRoomService;

        public ChatRoomController(IChatRoomService chatRoomService)
        {
            _chatRoomService = chatRoomService;
        }

        /// <summary>
        /// [Staff/Admin] Get all active chat rooms
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetActiveRooms()
        {
            var rooms = await _chatRoomService.GetActiveRoomsAsync();
            return Ok(rooms);
        }

        /// <summary>
        /// [Any authenticated user] Get rooms for the current customer
        /// </summary>
        [HttpGet("my-rooms")]
        [Authorize]
        public async Task<IActionResult> GetMyRooms()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var rooms = await _chatRoomService.GetCustomerRoomsAsync(userId.Value);
            return Ok(rooms);
        }

        /// <summary>
        /// Get a specific room with full message history
        /// </summary>
        [HttpGet("{roomId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetRoom(Guid roomId)
        {
            var room = await _chatRoomService.GetRoomAsync(roomId);
            if (room == null) return NotFound();
            return Ok(room);
        }

        /// <summary>
        /// [Staff/Admin] Toggle AI for a specific room
        /// </summary>
        [HttpPatch("{roomId:guid}/ai")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> ToggleAI(Guid roomId, [FromQuery] bool enabled)
        {
            await _chatRoomService.ToggleAIAsync(roomId, enabled);
            return Ok(new { RoomId = roomId, IsAIEnabled = enabled });
        }

        /// <summary>
        /// [Staff/Admin] Close a chat room
        /// </summary>
        [HttpPatch("{roomId:guid}/close")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> CloseRoom(Guid roomId)
        {
            await _chatRoomService.CloseRoomAsync(roomId);
            return Ok(new { Message = "Room closed." });
        }

        // ── Helper ───────────────────────────────────────────────────────────

        private Guid? GetCurrentUserId()
        {
            var sub = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}

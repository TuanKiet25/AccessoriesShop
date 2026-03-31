using AccessoriesShop.Application.IServices;
using AccessoriesShop.Web.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccessoriesShop.Web.Controllers
{
    public class ChatRoomController : MyBaseController
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatRoomController(IChatRoomService chatRoomService, IHubContext<ChatHub> hubContext)
        {
            _chatRoomService = chatRoomService;
            _hubContext = hubContext;
        }

        // ── Customer ─────────────────────────────────────────────────────────

        /// <summary>
        /// Customer creates a new chat room. Returns the room info.
        /// After calling this, connect to SignalR and call JoinRoom(roomId).
        /// </summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRoom()
        {
            var customerId = GetCurrentUserId();
            if (customerId == null) return Unauthorized();

            var room = await _chatRoomService.CreateRoomAsync(customerId.Value);
            return Ok(room);
        }

        /// <summary>
        /// Get all rooms belonging to the current customer.
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

        // ── Staff / Admin ─────────────────────────────────────────────────────

        /// <summary>
        /// [Staff/Admin] Get all active chat rooms (waiting or being handled).
        /// </summary>
        [HttpGet("active")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> GetActiveRooms()
        {
            var rooms = await _chatRoomService.GetActiveRoomsAsync();
            return Ok(rooms);
        }

        /// <summary>
        /// [Staff/Admin] Staff enters a room — AI turns off, room becomes HandledByStaff.
        /// Pushes a real-time "StaffJoined" event to everyone in the room via SignalR.
        /// After calling this, call JoinRoom(roomId) on the SignalR hub.
        /// </summary>
        [HttpPost("{roomId:guid}/join")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> StaffJoinRoom(Guid roomId)
        {
            var staffId = GetCurrentUserId();
            if (staffId == null) return Unauthorized();

            await _chatRoomService.StaffJoinRoomAsync(roomId, staffId.Value);
            var room = await _chatRoomService.GetRoomAsync(roomId);

            // Notify everyone already in the room
            await _hubContext.Clients.Group(ChatHub.RoomGroup(roomId))
                .SendAsync("StaffJoined", new { RoomId = roomId, StaffId = staffId, StaffName = room?.ActiveStaffName });

            return Ok(room);
        }

        /// <summary>
        /// [Staff/Admin] Staff leaves a room — AI turns back on, room goes back to Waiting.
        /// Pushes a real-time "StaffLeft" event to everyone in the room via SignalR.
        /// After calling this, call LeaveRoom(roomId) on the SignalR hub.
        /// </summary>
        [HttpPost("{roomId:guid}/leave")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> StaffLeaveRoom(Guid roomId)
        {
            var staffId = GetCurrentUserId();
            if (staffId == null) return Unauthorized();

            await _chatRoomService.StaffLeaveRoomAsync(roomId, staffId.Value);

            // Notify everyone in the room
            await _hubContext.Clients.Group(ChatHub.RoomGroup(roomId))
                .SendAsync("StaffLeft", new { RoomId = roomId });

            return Ok(new { Message = "Left room. AI re-enabled." });
        }

        /// <summary>
        /// [Staff/Admin] Manually toggle AI reply for a room.
        /// Pushes a real-time "AIStatusChanged" event to everyone in the room via SignalR.
        /// </summary>
        [HttpPatch("{roomId:guid}/ai")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> ToggleAI(Guid roomId, [FromQuery] bool enabled)
        {
            await _chatRoomService.ToggleAIAsync(roomId, enabled);

            await _hubContext.Clients.Group(ChatHub.RoomGroup(roomId))
                .SendAsync("AIStatusChanged", new { RoomId = roomId, IsAIEnabled = enabled });

            return Ok(new { RoomId = roomId, IsAIEnabled = enabled });
        }

        /// <summary>
        /// [Staff/Admin] Close a chat room permanently.
        /// Pushes a real-time "RoomClosed" event to everyone in the room via SignalR.
        /// </summary>
        [HttpPatch("{roomId:guid}/close")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<IActionResult> CloseRoom(Guid roomId)
        {
            await _chatRoomService.CloseRoomAsync(roomId);

            await _hubContext.Clients.Group(ChatHub.RoomGroup(roomId))
                .SendAsync("RoomClosed", new { RoomId = roomId });

            return Ok(new { Message = "Room closed." });
        }

        // ── Shared ────────────────────────────────────────────────────────────

        /// <summary>
        /// Get a specific room with its full message history.
        /// </summary>
        [HttpGet("{roomId:guid}")]
        [Authorize]
        public async Task<IActionResult> GetRoom(Guid roomId)
        {
            var room = await _chatRoomService.GetRoomAsync(roomId);
            if (room == null) return NotFound();
            return Ok(room);
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

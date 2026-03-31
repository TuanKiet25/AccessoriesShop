using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.Interfaces.External;
using AccessoriesShop.Application.ViewModels.Requests;
using AccessoriesShop.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AccessoriesShop.Web.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IAIIntegrationService _aiService;
        private readonly ILogger<ChatHub> _logger;

        // Group name for all connected staff members
        private const string StaffGroup = "staff-online";

        public ChatHub(
            IChatRoomService chatRoomService,
            IAIIntegrationService aiService,
            ILogger<ChatHub> logger)
        {
            _chatRoomService = chatRoomService;
            _aiService = aiService;
            _logger = logger;
        }

        // ── Connection lifecycle ─────────────────────────────────────────────

        public override async Task OnConnectedAsync()
        {
            if (IsStaff())
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, StaffGroup);
                _logger.LogInformation("Staff {UserId} connected to ChatHub", GetUserId());
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (IsStaff())
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, StaffGroup);
            }
            await base.OnDisconnectedAsync(exception);
        }

        // ── Customer actions ─────────────────────────────────────────────────

        /// <summary>
        /// Customer creates a new chat room. Returns the room id to the caller
        /// and notifies all staff about the new room.
        /// </summary>
        public async Task CreateRoom()
        {
            var customerId = GetUserId();
            if (customerId == null)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                var room = await _chatRoomService.CreateRoomAsync(customerId.Value);

                // Add customer to this room's SignalR group
                await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(room.Id));

                // Notify caller with room details
                await Clients.Caller.SendAsync("RoomCreated", room);

                // Notify all staff of the new room
                await Clients.Group(StaffGroup).SendAsync("NewRoomAvailable", room);

                _logger.LogInformation("Customer {CustomerId} created room {RoomId}", customerId, room.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateRoom failed for customer {CustomerId}", customerId);
                await Clients.Caller.SendAsync("Error", "Failed to create chat room.");
            }
        }

        /// <summary>
        /// Customer sends a message. If AI is enabled, an AI reply is generated automatically.
        /// </summary>
        public async Task CustomerSendMessage(Guid roomId, string message)
        {
            var customerId = GetUserId();
            if (customerId == null)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                // Save the customer's message
                var customerMsg = await _chatRoomService.SaveMessageAsync(
                    roomId, customerId.Value, message, MessageUserType.Customer);

                // Broadcast to everyone in the room
                await Clients.Group(RoomGroup(roomId)).SendAsync("ReceiveMessage", customerMsg);

                // Check if AI should reply
                var room = await _chatRoomService.GetRoomAsync(roomId);
                if (room == null)
                {
                    await Clients.Caller.SendAsync("Error", "Room not found.");
                    return;
                }

                if (room.IsAIEnabled && room.Status != ChatRoomStatus.Closed)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var aiResult = await _aiService.GetChatResponseAsync(message);
                            if (aiResult.IsSuccess && !string.IsNullOrWhiteSpace(aiResult.Response))
                            {
                                var aiMsg = await _chatRoomService.SaveMessageAsync(
                                    roomId, null, aiResult.Response!, MessageUserType.AI);

                                await Clients.Group(RoomGroup(roomId)).SendAsync("ReceiveMessage", aiMsg);
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "AI reply failed for room {RoomId}", roomId);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CustomerSendMessage failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to send message.");
            }
        }

        // ── Staff actions ────────────────────────────────────────────────────

        /// <summary>
        /// Staff joins a room. AI is disabled and all participants are notified.
        /// </summary>
        public async Task StaffJoinRoom(Guid roomId)
        {
            var staffId = GetUserId();
            if (staffId == null || !IsStaff())
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
                await _chatRoomService.StaffJoinRoomAsync(roomId, staffId.Value);

                // Notify everyone in the room
                await Clients.Group(RoomGroup(roomId))
                    .SendAsync("StaffJoined", new { RoomId = roomId, StaffId = staffId });

                // Confirm to staff caller
                var room = await _chatRoomService.GetRoomAsync(roomId);
                await Clients.Caller.SendAsync("RoomJoined", room);

                _logger.LogInformation("Staff {StaffId} joined room {RoomId}", staffId, roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StaffJoinRoom failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to join room.");
            }
        }

        /// <summary>
        /// Staff leaves a room. AI is re-enabled and participants are notified.
        /// </summary>
        public async Task StaffLeaveRoom(Guid roomId)
        {
            var staffId = GetUserId();
            if (staffId == null || !IsStaff())
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                await _chatRoomService.StaffLeaveRoomAsync(roomId, staffId.Value);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(roomId));

                await Clients.Group(RoomGroup(roomId))
                    .SendAsync("StaffLeft", new { RoomId = roomId, StaffId = staffId });

                _logger.LogInformation("Staff {StaffId} left room {RoomId}", staffId, roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StaffLeaveRoom failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to leave room.");
            }
        }

        /// <summary>
        /// Staff sends a message to the customer.
        /// </summary>
        public async Task StaffSendMessage(Guid roomId, string message)
        {
            var staffId = GetUserId();
            if (staffId == null || !IsStaff())
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                var staffMsg = await _chatRoomService.SaveMessageAsync(
                    roomId, staffId.Value, message, MessageUserType.Staff);

                await Clients.Group(RoomGroup(roomId)).SendAsync("ReceiveMessage", staffMsg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "StaffSendMessage failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to send message.");
            }
        }

        /// <summary>
        /// Staff manually enables or disables AI reply for a specific room.
        /// </summary>
        public async Task ToggleAI(Guid roomId, bool enabled)
        {
            if (!IsStaff())
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                await _chatRoomService.ToggleAIAsync(roomId, enabled);

                await Clients.Group(RoomGroup(roomId))
                    .SendAsync("AIStatusChanged", new { RoomId = roomId, IsAIEnabled = enabled });

                _logger.LogInformation("AI toggled to {Enabled} for room {RoomId}", enabled, roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ToggleAI failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to toggle AI.");
            }
        }

        /// <summary>
        /// Staff or customer subscribes to room updates (e.g. on page reload).
        /// Returns full message history.
        /// </summary>
        public async Task JoinRoomGroup(Guid roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
            var room = await _chatRoomService.GetRoomAsync(roomId);
            await Clients.Caller.SendAsync("RoomHistory", room);
        }

        /// <summary>
        /// Staff closes a chat room.
        /// </summary>
        public async Task CloseRoom(Guid roomId)
        {
            if (!IsStaff())
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                await _chatRoomService.CloseRoomAsync(roomId);
                await Clients.Group(RoomGroup(roomId)).SendAsync("RoomClosed", new { RoomId = roomId });
                _logger.LogInformation("Room {RoomId} closed by staff", roomId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseRoom failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to close room.");
            }
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private Guid? GetUserId()
        {
            var sub = Context.User?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                   ?? Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return Guid.TryParse(sub, out var id) ? id : null;
        }

        private bool IsStaff()
        {
            var role = Context.User?.FindFirst("role")?.Value;
            return role == nameof(Role.Staff) || role == nameof(Role.Admin);
        }

        private static string RoomGroup(Guid roomId) => $"room-{roomId}";
    }
}

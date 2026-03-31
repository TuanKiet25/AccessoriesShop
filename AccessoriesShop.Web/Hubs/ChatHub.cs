using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.Interfaces.External;
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

        public ChatHub(
            IChatRoomService chatRoomService,
            IAIIntegrationService aiService,
            ILogger<ChatHub> logger)
        {
            _chatRoomService = chatRoomService;
            _aiService = aiService;
            _logger = logger;
        }

        // ── Group subscription ────────────────────────────────────────────────

        /// <summary>
        /// Subscribe to a room's real-time messages. Call this after entering a room via REST.
        /// </summary>
        public async Task JoinRoom(Guid roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        }

        /// <summary>
        /// Unsubscribe from a room's real-time messages.
        /// </summary>
        public async Task LeaveRoom(Guid roomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, RoomGroup(roomId));
        }

        // ── Messaging ─────────────────────────────────────────────────────────

        /// <summary>
        /// Send a message to a room. Works for both customers and staff.
        /// If the room has AI enabled and the sender is a customer, an AI reply is generated.
        /// </summary>
        public async Task SendMessage(Guid roomId, string message)
        {
            var senderId = GetUserId();
            if (senderId == null)
            {
                await Clients.Caller.SendAsync("Error", "Unauthorized");
                return;
            }

            try
            {
                var room = await _chatRoomService.GetRoomAsync(roomId);
                if (room == null)
                {
                    await Clients.Caller.SendAsync("Error", "Room not found.");
                    return;
                }

                if (room.Status == ChatRoomStatus.Closed)
                {
                    await Clients.Caller.SendAsync("Error", "This room is closed.");
                    return;
                }

                // Determine sender type from JWT role
                var userType = IsStaff() ? MessageUserType.Staff : MessageUserType.Customer;

                // Save and broadcast the message
                var savedMsg = await _chatRoomService.SaveMessageAsync(roomId, senderId.Value, message, userType);
                await Clients.Group(RoomGroup(roomId)).SendAsync("ReceiveMessage", savedMsg);

                // AI auto-reply only when a customer sends and AI is enabled
                if (userType == MessageUserType.Customer && room.IsAIEnabled)
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
                _logger.LogError(ex, "SendMessage failed for room {RoomId}", roomId);
                await Clients.Caller.SendAsync("Error", "Failed to send message.");
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

        public static string RoomGroup(Guid roomId) => $"room-{roomId}";
    }
}

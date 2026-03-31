using AccessoriesShop.Application.IServices;
using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Application.ViewModels.Responses;
using AccessoriesShop.Domain.Entities;
using AccessoriesShop.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace AccessoriesShop.Application.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _chatRoomRepo;
        private readonly IChatMessageRepository _chatMessageRepo;
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ChatRoomService> _logger;

        public ChatRoomService(
            IChatRoomRepository chatRoomRepo,
            IChatMessageRepository chatMessageRepo,
            IUnitOfWork uow,
            ILogger<ChatRoomService> logger)
        {
            _chatRoomRepo = chatRoomRepo;
            _chatMessageRepo = chatMessageRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<ChatRoomResponse> CreateRoomAsync(Guid customerId)
        {
            var room = new ChatRoom
            {
                CustomerId = customerId,
                IsAIEnabled = true,
                Status = ChatRoomStatus.Waiting,
                CreatedAt = DateTime.UtcNow
            };

            await _chatRoomRepo.AddAsync(room);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("ChatRoom {RoomId} created for customer {CustomerId}", room.Id, customerId);

            return MapRoomToResponse(room, new List<ChatMessage>());
        }

        public async Task<ChatRoomResponse?> GetRoomAsync(Guid roomId)
        {
            var room = await _chatRoomRepo.GetRoomWithMessagesAsync(roomId);
            if (room == null) return null;

            return MapRoomToResponse(room, room.Messages.ToList());
        }

        public async Task<List<ChatRoomResponse>> GetActiveRoomsAsync()
        {
            var rooms = await _chatRoomRepo.GetActiveRoomsAsync();
            return rooms.Select(r => MapRoomToResponse(r, new List<ChatMessage>())).ToList();
        }

        public async Task<List<ChatRoomResponse>> GetCustomerRoomsAsync(Guid customerId)
        {
            var rooms = await _chatRoomRepo.GetCustomerRoomsAsync(customerId);
            return rooms.Select(r => MapRoomToResponse(r, new List<ChatMessage>())).ToList();
        }

        public async Task StaffJoinRoomAsync(Guid roomId, Guid staffId)
        {
            var room = await _chatRoomRepo.GetByIdAsync(roomId)
                ?? throw new KeyNotFoundException($"ChatRoom {roomId} not found.");

            room.ActiveStaffId = staffId;
            room.Status = ChatRoomStatus.HandledByStaff;
            room.IsAIEnabled = false;

            await _chatRoomRepo.UpdateAsync(room);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Staff {StaffId} joined ChatRoom {RoomId}. AI disabled.", staffId, roomId);
        }

        public async Task StaffLeaveRoomAsync(Guid roomId, Guid staffId)
        {
            var room = await _chatRoomRepo.GetByIdAsync(roomId)
                ?? throw new KeyNotFoundException($"ChatRoom {roomId} not found.");

            // Only re-enable AI if it's the same staff leaving
            if (room.ActiveStaffId == staffId)
            {
                room.ActiveStaffId = null;
                room.Status = ChatRoomStatus.Waiting;
                room.IsAIEnabled = true;
            }

            await _chatRoomRepo.UpdateAsync(room);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("Staff {StaffId} left ChatRoom {RoomId}. AI re-enabled.", staffId, roomId);
        }

        public async Task ToggleAIAsync(Guid roomId, bool enabled)
        {
            var room = await _chatRoomRepo.GetByIdAsync(roomId)
                ?? throw new KeyNotFoundException($"ChatRoom {roomId} not found.");

            room.IsAIEnabled = enabled;

            await _chatRoomRepo.UpdateAsync(room);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("ChatRoom {RoomId} AI toggled to {Enabled}.", roomId, enabled);
        }

        public async Task<ChatMessageResponse> SaveMessageAsync(Guid roomId, Guid? senderId, string content, MessageUserType userType)
        {
            var message = new ChatMessage
            {
                RoomId = roomId,
                SenderId = senderId,
                Content = content,
                UserType = userType,
                CreatedAt = DateTime.UtcNow
            };

            await _chatMessageRepo.AddAsync(message);
            await _uow.SaveChangesAsync();

            return MapMessageToResponse(message, null);
        }

        public async Task CloseRoomAsync(Guid roomId)
        {
            var room = await _chatRoomRepo.GetByIdAsync(roomId)
                ?? throw new KeyNotFoundException($"ChatRoom {roomId} not found.");

            room.Status = ChatRoomStatus.Closed;
            room.ClosedAt = DateTime.UtcNow;
            room.IsAIEnabled = false;

            await _chatRoomRepo.UpdateAsync(room);
            await _uow.SaveChangesAsync();

            _logger.LogInformation("ChatRoom {RoomId} closed.", roomId);
        }

        // ── Mapping helpers ─────────────────────────────────────────────────────

        private static ChatRoomResponse MapRoomToResponse(ChatRoom room, List<ChatMessage> messages)
        {
            return new ChatRoomResponse
            {
                Id = room.Id,
                Name = room.Name,
                CustomerId = room.CustomerId,
                CustomerName = room.Customer?.Username,
                ActiveStaffId = room.ActiveStaffId,
                ActiveStaffName = room.ActiveStaff?.Username,
                Status = room.Status,
                IsAIEnabled = room.IsAIEnabled,
                CreatedAt = room.CreatedAt,
                ClosedAt = room.ClosedAt,
                Messages = messages.Select(m => MapMessageToResponse(m, m.Sender)).ToList()
            };
        }

        private static ChatMessageResponse MapMessageToResponse(ChatMessage msg, Account? sender)
        {
            return new ChatMessageResponse
            {
                Id = msg.Id,
                RoomId = msg.RoomId,
                SenderId = msg.SenderId,
                SenderName = sender?.Username ?? (msg.UserType == MessageUserType.AI ? "AI Assistant" : null),
                Content = msg.Content,
                UserType = msg.UserType,
                CreatedAt = msg.CreatedAt
            };
        }
    }
}

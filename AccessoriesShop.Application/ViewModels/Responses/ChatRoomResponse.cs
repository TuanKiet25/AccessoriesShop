using AccessoriesShop.Domain.Enums;

namespace AccessoriesShop.Application.ViewModels.Responses
{
    public class ChatRoomResponse
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? ActiveStaffId { get; set; }
        public string? ActiveStaffName { get; set; }
        public ChatRoomStatus Status { get; set; }
        public bool IsAIEnabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public List<ChatMessageResponse> Messages { get; set; } = new();
        public int UnreadCount { get; set; }
    }

    public class ChatMessageResponse
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid? SenderId { get; set; }
        public string? SenderName { get; set; }
        public string Content { get; set; } = string.Empty;
        public MessageUserType UserType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

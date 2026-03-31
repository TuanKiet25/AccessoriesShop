using AccessoriesShop.Domain.Enums;

namespace AccessoriesShop.Domain.Entities
{
    public class ChatRoom
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Optional display name for the chat room
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The customer who owns this chat room
        /// </summary>
        public Guid CustomerId { get; set; }

        /// <summary>
        /// The staff member currently handling this room (null = no staff)
        /// </summary>
        public Guid? ActiveStaffId { get; set; }

        public ChatRoomStatus Status { get; set; } = ChatRoomStatus.Waiting;

        /// <summary>
        /// Whether AI auto-reply is enabled for this room
        /// Defaults to true. Turns off when staff joins, back on when staff leaves.
        /// Staff can also toggle manually.
        /// </summary>
        public bool IsAIEnabled { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        public virtual Account Customer { get; set; } = null!;
        public virtual Account? ActiveStaff { get; set; }
        public virtual ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}

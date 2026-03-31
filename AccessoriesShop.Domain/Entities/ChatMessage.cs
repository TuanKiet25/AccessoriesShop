using AccessoriesShop.Domain.Enums;

namespace AccessoriesShop.Domain.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid RoomId { get; set; }

        /// <summary>
        /// Sender account ID. Null if this message was sent by AI.
        /// </summary>
        public Guid? SenderId { get; set; }

        public string Content { get; set; } = string.Empty;
        public MessageUserType UserType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Account? Sender { get; set; }
        public virtual ChatRoom Room { get; set; } = null!;
    }
}

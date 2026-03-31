using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Application.Repositories
{
    public interface IChatMessageRepository : IGenericRepository<ChatMessage>
    {
        /// <summary>
        /// Get all messages for a room ordered by time ascending
        /// </summary>
        Task<List<ChatMessage>> GetRoomMessagesAsync(Guid roomId);
    }
}

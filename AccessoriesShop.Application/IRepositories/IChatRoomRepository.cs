using AccessoriesShop.Domain.Entities;

namespace AccessoriesShop.Application.Repositories
{
    public interface IChatRoomRepository : IGenericRepository<ChatRoom>
    {
        /// <summary>
        /// Get room with messages and sender info included
        /// </summary>
        Task<ChatRoom?> GetRoomWithMessagesAsync(Guid roomId);

        /// <summary>
        /// Get all active rooms (Waiting or HandledByStaff) ordered by latest activity
        /// </summary>
        Task<List<ChatRoom>> GetActiveRoomsAsync();

        /// <summary>
        /// Get all rooms belonging to a customer
        /// </summary>
        Task<List<ChatRoom>> GetCustomerRoomsAsync(Guid customerId);
    }
}

using AccessoriesShop.Application.ViewModels.Responses;

namespace AccessoriesShop.Application.IServices
{
    public interface IChatRoomService
    {
        /// <summary>
        /// Create a new chat room for a customer
        /// </summary>
        Task<ChatRoomResponse> CreateRoomAsync(Guid customerId);

        /// <summary>
        /// Get a single room with its message history
        /// </summary>
        Task<ChatRoomResponse?> GetRoomAsync(Guid roomId);

        /// <summary>
        /// Get all active rooms (for staff dashboard)
        /// </summary>
        Task<List<ChatRoomResponse>> GetActiveRoomsAsync();

        /// <summary>
        /// Get all rooms for a specific customer
        /// </summary>
        Task<List<ChatRoomResponse>> GetCustomerRoomsAsync(Guid customerId);

        /// <summary>
        /// Record a staff joining a room — disables AI, sets status to HandledByStaff
        /// </summary>
        Task StaffJoinRoomAsync(Guid roomId, Guid staffId);

        /// <summary>
        /// Record a staff leaving a room — re-enables AI, sets status back to Waiting
        /// </summary>
        Task StaffLeaveRoomAsync(Guid roomId, Guid staffId);

        /// <summary>
        /// Manually toggle AI reply for a specific room
        /// </summary>
        Task ToggleAIAsync(Guid roomId, bool enabled);

        /// <summary>
        /// Save an incoming message and return the saved DTO
        /// </summary>
        Task<ChatMessageResponse> SaveMessageAsync(Guid roomId, Guid? senderId, string content, Domain.Enums.MessageUserType userType);

        /// <summary>
        /// Close a chat room
        /// </summary>
        Task CloseRoomAsync(Guid roomId);
    }
}

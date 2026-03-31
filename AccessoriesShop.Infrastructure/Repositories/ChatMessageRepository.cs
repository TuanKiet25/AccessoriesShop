using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class ChatMessageRepository : GenericRepository<ChatMessage>, IChatMessageRepository
    {
        public ChatMessageRepository(AppDbContext context) : base(context) { }

        public async Task<List<ChatMessage>> GetRoomMessagesAsync(Guid roomId)
        {
            return await _context.ChatMessages
                .Include(m => m.Sender)
                .Where(m => m.RoomId == roomId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}

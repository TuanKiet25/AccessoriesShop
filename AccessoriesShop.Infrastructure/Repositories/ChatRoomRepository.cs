using AccessoriesShop.Application.Repositories;
using AccessoriesShop.Domain.Entities;
using AccessoriesShop.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace AccessoriesShop.Infrastructure.Repositories
{
    public class ChatRoomRepository : GenericRepository<ChatRoom>, IChatRoomRepository
    {
        public ChatRoomRepository(AppDbContext context) : base(context) { }

        public async Task<ChatRoom?> GetRoomWithMessagesAsync(Guid roomId)
        {
            return await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.ActiveStaff)
                .Include(r => r.Messages.OrderBy(m => m.CreatedAt))
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<List<ChatRoom>> GetActiveRoomsAsync()
        {
            return await _context.ChatRooms
                .Include(r => r.Customer)
                .Include(r => r.ActiveStaff)
                .Where(r => r.Status != ChatRoomStatus.Closed)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<ChatRoom>> GetCustomerRoomsAsync(Guid customerId)
        {
            return await _context.ChatRooms
                .Include(r => r.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }
    }
}

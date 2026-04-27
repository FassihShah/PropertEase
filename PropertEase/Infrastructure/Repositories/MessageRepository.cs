using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetReceivedMessagesAsync(string recipientId)
        {
            return await _context.Messages
                .Where(m => m.RecipientId == recipientId)
                .OrderByDescending(m => m.SentTime)
                .ToListAsync();
        }

        public async Task<List<Message>> GetSentMessagesAsync(string senderId)
        {
            return await _context.Messages
                .Where(m => m.SenderId == senderId)
                .OrderByDescending(m => m.SentTime)
                .ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}

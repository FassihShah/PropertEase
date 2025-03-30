using Domain.Interfaces;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return await _context.Messages.Where(m => m.RecipientId == recipientId).ToListAsync();
        }

        public async Task<List<Message>> GetSentMessagesAsync(string senderId)
        {
            return await _context.Messages.Where(m => m.SenderId == senderId).ToListAsync();
        }

        public async Task AddAsync(Message message)
        {
            await _context.Messages.AddAsync(message);
            await _context.SaveChangesAsync();
        }
    }
}

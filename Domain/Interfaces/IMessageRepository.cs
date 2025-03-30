using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetReceivedMessagesAsync(string recipientId);
        Task<List<Message>> GetSentMessagesAsync(string senderId);
        Task AddAsync(Message message);
    }
}

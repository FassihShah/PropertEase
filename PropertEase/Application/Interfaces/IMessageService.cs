using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetReceivedMessagesAsync(string userId);
        Task<List<Message>> GetSentMessagesAsync(string userId);
        Task SendMessageAsync(Message message);
    }
}

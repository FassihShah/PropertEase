using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        public async Task<List<Message>> GetReceivedMessagesAsync(string userId)
        {
            return await _messageRepository.GetReceivedMessagesAsync(userId);
        }

        public async Task<List<Message>> GetSentMessagesAsync(string userId)
        {
            return await _messageRepository.GetSentMessagesAsync(userId);
        }

        public async Task SendMessageAsync(Message message)
        {
            await _messageRepository.AddAsync(message);
        }
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Identity;
using Domain.Entities;
using PropertEase.Hubs;
using Microsoft.AspNetCore.SignalR;
using Application.Interfaces;
using System.Security.Claims;

namespace PropertEase.Controllers
{
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly IPropertyService _propertyService;
        private readonly IUserService _userService;
        private readonly IHubContext<MessagingHub> _hubContext;

        public MessageController(IMessageService messageService, IUserService userService, IHubContext<MessagingHub> hubContext, IPropertyService propertyService)
        {
            _messageService = messageService;
            _userService = userService;
            _hubContext = hubContext;
            _propertyService = propertyService;
        }

        public async Task<IActionResult> ReceivedMessages()
        {
            var userId = await _userService.GetCurrentUserIdAsync();

            List<Message> messages = await _messageService.GetReceivedMessagesAsync(userId);
            return View(messages);
        }

        public async Task<IActionResult> SentMessages()
        {
            var userId = await _userService.GetCurrentUserIdAsync();

            List<Message> messages = await _messageService.GetSentMessagesAsync(userId);
            return View(messages);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(int PropertyId, string FullName, string Email, string MessageContent)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "User");
            }

            var userId = await _userService.GetCurrentUserIdAsync();

            var recipientId = (await _propertyService.GetByIdAsync(PropertyId)).SellerId;

            var message = new Message
            {
                PropertyId = PropertyId,
                Content = MessageContent,
                SenderId = userId,
                RecipientId = recipientId,
                SentTime = DateTime.Now
            };

            await _messageService.SendMessageAsync(message);
            return RedirectToAction("PropertyDetails", "Property", new { id = PropertyId });
        }
    }
}

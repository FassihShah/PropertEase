using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PropertEase.Hubs;

namespace PropertEase.Controllers
{
    [Authorize]
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
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var messages = await _messageService.GetReceivedMessagesAsync(userId);
            return View(messages);
        }

        public async Task<IActionResult> SentMessages()
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var messages = await _messageService.GetSentMessagesAsync(userId);
            return View(messages);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(int PropertyId, string FullName, string Email, string MessageContent)
        {
            var userId = await _userService.GetCurrentUserIdAsync();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Challenge();
            }

            var property = await _propertyService.GetByIdAsync(PropertyId);
            if (property == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(MessageContent))
            {
                return RedirectToAction("PropertyDetails", "Property", new { id = PropertyId });
            }

            var message = new Message
            {
                PropertyId = PropertyId,
                Content = MessageContent.Trim(),
                SenderId = userId,
                RecipientId = property.SellerId,
                SentTime = DateTime.UtcNow
            };

            await _messageService.SendMessageAsync(message);
            await _hubContext.Clients.User(property.SellerId).SendAsync("ReceiveMessage", message.Content);

            return RedirectToAction("PropertyDetails", "Property", new { id = PropertyId });
        }
    }
}

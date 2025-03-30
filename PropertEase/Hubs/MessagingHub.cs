using Microsoft.AspNetCore.SignalR;

namespace PropertEase.Hubs
{
	public class MessagingHub:Hub
	{
		public async Task NotifyNewMessage(string receiverId, string senderName, string propertyTitle)
		{
			await Clients.User(receiverId).SendAsync("NewMessageReceived", senderName, propertyTitle);
		}
	}
}

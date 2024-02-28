using chat_app_service.Domain.Entities;
using Microsoft.AspNetCore.SignalR;

namespace chat_app_service.Core.Hubs;

public class MessageHub : Hub
{
    public async Task SendMessage(Message message)
    {
        await Clients.All.SendAsync("ReceiveMessage", message);
    }
}

using chat_app_service.Infrastructure.Databases;
using Microsoft.AspNetCore.SignalR;
namespace chat_app_service.Core.Hubs;

public class MessageHub : Hub
{
    private readonly AppChatDbContext _databaseContext;

    public MessageHub(AppChatDbContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task UpdateStatus(int userId)
    {
        var user = await _databaseContext.Users.FindAsync(userId);
        if (user != null)
        {
            user.Status = 1;
            await _databaseContext.SaveChangesAsync();

            // Broadcast updated status to connected clients (if using SignalR)
            await Clients.All.SendAsync("userStatusUpdated", user);
        }
    }
    //public override async Task OnConnectedAsync()
    //{
    //    await Clients.All.SendAsync("broadcastMessage", "system", $"{Context.ConnectionId} joined the conversation");
    //    await base.OnConnectedAsync();
    //}

    //public async Task Ping()
    //{
    //    await Clients.All.SendAsync("broadcastMessage", "system", $"{Context.ConnectionId} joined the conversation");
    //}

    //public async Task AskStockPrice(String Stock)
    //{
    //    await Clients.All.SendAsync($"Receive: Stock:{Stock} ,Pricce:{1000},Volum :{2000}");
    //}
    //public async Task SendMessage(MessageDTO message)
    //{
    //    await Clients.All.SendAsync("ReceiveMessage", message);
    //}
    //public override async Task OnDisconnectedAsync(System.Exception exception)
    //{
    //    await Clients.All.SendAsync("broadcastMessage", "system", $"{Context.ConnectionId} left the conversation");
    //    await base.OnDisconnectedAsync(exception);
    //}

}

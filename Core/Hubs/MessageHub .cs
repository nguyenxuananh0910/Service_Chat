using chat_app_service.Infrastructure.Databases;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
namespace chat_app_service.Core.Hubs;

public class MessageHub : Hub
{
    private readonly AppChatDbContext _context; // Inject DbContext

    public MessageHub(AppChatDbContext context)
    {
        _context = context;
    }

    public async Task UpdateUserStatus(int userId, bool status)
    {
        // Retrieve the user from the database
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);

        // Check if the user exists
        if (user != null)
        {
            // Update the user's status
            user.Status = status;

            // Save changes to the database
            await _context.SaveChangesAsync();
        }

        // Broadcast the updated status to all clients
        await Clients.All.SendAsync("UpdateUserStatus", userId, status);
    }
}

using chat_app_service.Infrastructure.Databases;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
namespace chat_app_service.Core.Hubs;

public class MessageHub : Hub
{
    private readonly AppChatDbContext _dbContext; // Inject DbContext

    public MessageHub(AppChatDbContext dbContext)
    {
        _dbContext = dbContext;

    }


    //public async Task UpdateUserStatus(int userId, bool status)
    //{
    //    // Retrieve the user from the database
    //    var user = await _context.Users.FirstOrDefaultAsync(u => u.Userid == userId);

    //    // Check if the user exists
    //    if (user != null)
    //    {
    //        // Update the user's status
    //        user.Status = status;

    //        // Save changes to the database
    //        await _context.SaveChangesAsync();
    //    }

    //    // Broadcast the updated status to all clients
    //    await Clients.All.SendAsync("UpdateUserStatus", userId, status);
    //}
    public override async Task OnConnectedAsync()
    {
        //var connection = Context.ConnectionId;
        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier); // Get connection ID
        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;
            // Now you have the user ID, you can use it as needed
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Userid == int.Parse(userId));
            if (user != null)
            {
                // Update the user's status
                user.Status = true;

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
            await Clients.All.SendAsync("UpdateUserStatus", userId, true);
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var userIdClaim = Context.User.FindFirst(ClaimTypes.NameIdentifier); // Get connection ID
        if (userIdClaim != null)
        {
            var userId = userIdClaim.Value;
            // Now you have the user ID, you can use it as needed
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Userid == int.Parse(userId));
            if (user != null)
            {
                // Update the user's status
                user.Status = false;

                // Save changes to the database
                await _dbContext.SaveChangesAsync();
            }
            await Clients.All.SendAsync("UpdateUserStatus", userId, false);
        }

        await base.OnDisconnectedAsync(exception);
    }


}

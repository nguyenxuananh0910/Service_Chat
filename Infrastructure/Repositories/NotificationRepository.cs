using FirebaseAdmin.Messaging;

namespace chat_app_service.Infrastructure.Repositories;

public class NotificationRepository
{
    public async Task SendNotificationAsync(string deviceToken, string title, string body)
    {
        var message = new Message()
        {
            Notification = new Notification()
            {
                Title = title,
                Body = body,
            },
            Token = deviceToken,
        };

        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        System.Console.WriteLine("Successfully sent message: " + response);
    }
}

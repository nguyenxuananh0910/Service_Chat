namespace chat_app_service.Core.Authentication;

//public class NameUserIdProvider : IUserIdProvider
//{
//    public string? GetUserId(HubConnectionContext context)
//    {
//        var claimsPrincipal = context.User;
//        if (claimsPrincipal?.HasClaim(c => c.Type == ClaimTypes.NameIdentifier) == true)
//        {
//            return claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
//        }
//        return null;

//    }
//}

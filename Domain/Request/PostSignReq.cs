namespace chat_app_service.Domain.Request;

public class PostSignReq
{
    public string? Username { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

}

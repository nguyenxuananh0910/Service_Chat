namespace chat_app_service.Domain.DTOs;

public class LoginDTO
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string token { get; set; } = string.Empty;

}

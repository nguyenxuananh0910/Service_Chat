namespace chat_app_service.Domain.DTOs;

public class SignDTO
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateTime? CreatedAt { get; set; }
}

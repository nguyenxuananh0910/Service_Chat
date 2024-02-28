namespace chat_app_service.Domain.DTOs.Core;

public class UserDTO
{
    public int Userid { get; set; }

    public string? Username { get; set; }

    public string? Fullname { get; set; }

    public string? Email { get; set; }

    public int? Status { get; set; }

}

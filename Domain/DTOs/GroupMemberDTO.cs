namespace chat_app_service.Domain.DTOs;

public class GroupMemberDTO
{
    public int? GroupId { get; set; }

    public int? Userid { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    public UserDTO? Users { get; set; } = new UserDTO();
}

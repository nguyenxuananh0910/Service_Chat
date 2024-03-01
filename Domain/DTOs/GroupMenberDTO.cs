namespace chat_app_service.Domain.DTOs;

public class GroupMenberDTO
{
    public int? GroupsId { get; set; }

    public int? Userid { get; set; }

    public DateTime? JoinedAt { get; set; }

    public DateTime? LeftAt { get; set; }

    //public UserDTO? Users { get; set; } = new UserDTO();
}

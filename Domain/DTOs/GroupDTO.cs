namespace chat_app_service.Domain.DTOs;

public class GroupDTO
{
    public int GroupsId { get; set; }

    public string? Name { get; set; }

    public int? LastMessageId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public List<GroupMenberDTO> Menbers { get; set; } = new List<GroupMenberDTO>();
}

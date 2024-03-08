namespace chat_app_service.Domain.DTOs;

public class GroupDTO
{
    public int GroupId { get; set; }

    public string? Name { get; set; }

    public int? LastMessageId { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public List<GroupMemberDTO> Members { get; set; } = new List<GroupMemberDTO>();

    public MessageDTO? LastMessage { get; set; } = new MessageDTO();
}

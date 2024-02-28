namespace chat_app_service.Domain.DTOs;

public class MessageDTO
{
    public int MessageId { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public int? GroupId { get; set; }
    public string? Content { get; set; }

    public int? Type { get; set; }

    public DateTime? CreatedAt { get; set; }
}

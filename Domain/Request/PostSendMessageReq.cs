namespace chat_app_service.Domain.Request;

public class PostSendMessageReq
{
    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public int? GroupId { get; set; }

    public string? Content { get; set; }

    public int? Type { get; set; }


}

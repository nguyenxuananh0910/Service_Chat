namespace chat_app_service.Domain.Request;

public class PostCreateGroupReq
{
    public string? GroupName { get; set; }

    public int? CreatedBy { get; set; }

    public List<GroupMenberReq>? Members { get; set; }
}

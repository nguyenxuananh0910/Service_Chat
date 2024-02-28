namespace chat_app_service.Domain.Request;

public class PostCreateGroupReq
{
    public string? Name { get; set; }

    public int? CreatedBy { get; set; }

    public List<GroupMenberReq>? Menber { get; set; }
}

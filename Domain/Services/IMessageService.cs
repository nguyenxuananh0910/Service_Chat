using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Request;

namespace chat_app_service.Domain.Services;

public interface IMessageService
{
    public Task<MessageDTO> SendMessage(PostSendMessageReq req);

    public Task<List<MessageDTO>> GetMessages(long groupId);

    public Task<List<MessageDTO>> GetMessagesBetweenUsers(long userIdA, long userIdB);
    ///Group

    public Task<GroupDTO> CreateGroup(PostCreateGroupReq req);
    public Task<List<GroupDTO>> GetGroups(long userId);
}

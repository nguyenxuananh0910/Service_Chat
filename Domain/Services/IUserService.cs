using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Request;

namespace chat_app_service.Domain.Services;

public interface IUserService
{
    public Task<LoginDTO> Login(PostLoginReq req);

    public Task<SignDTO> Sign(PostSignReq req);

    public Task<UserDTO> GetUser(long userId);
    public Task<List<UserDTO>> GetUsers(long userId);

    //public Task<string> UpdateUserStatus(long userId, bool status);
}

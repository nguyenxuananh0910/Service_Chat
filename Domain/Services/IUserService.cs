using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.DTOs.Core;
using chat_app_service.Domain.Request;

namespace chat_app_service.Domain.Services;

public interface IUserService
{
    public Task<LoginDTO> Login(PostLoginReq req);

    public Task<SignDTO> Sign(PostSignReq req);


    public Task<List<UserDTO>> GetUsers();
}

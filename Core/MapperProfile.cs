using AutoMapper;
using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.DTOs.Core;
using chat_app_service.Domain.Entities;
using chat_app_service.Domain.Request;


namespace chat_app_service.Core;

public class MapperProfile : Profile
{
    public MapperProfile()
    {
        CreateMap<User, LoginDTO>();
        CreateMap<LoginDTO, PostLoginReq>();
        CreateMap<User, SignDTO>();
        CreateMap<SignDTO, PostSignReq>();
        CreateMap<User, UserDTO>();
        CreateMap<Message, MessageDTO>();
        CreateMap<MessageDTO, PostSendMessageReq>();
        CreateMap<Group, GroupDTO>();
        CreateMap<GroupDTO, PostCreateGroupReq>();
        CreateMap<GroupMember, GroupMenberDTO>();
        CreateMap<GroupMenberDTO, GroupMenberReq>();
        CreateMap<GroupMenberReq, GroupMember>();
    }
}

using AutoMapper;
using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Entities;
using chat_app_service.Domain.Request;
using Microsoft.IdentityModel.Tokens;


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
        CreateMap<Group, GroupDTO>()
         .ForMember(dest => dest.Members,
           opt => opt.MapFrom((src, dest, i, context) =>
           {
               if (src.GroupMembers.IsNullOrEmpty()) return new List<GroupMemberDTO>();

               return context.Mapper.Map<List<GroupMemberDTO>>(src.GroupMembers);
           })).ForMember(dest => dest.LastMessage,
           opt => opt.MapFrom((src, dest, i, context) =>
           {
               return context.Mapper.Map<MessageDTO>(src.LastMessage);
           }));
        CreateMap<GroupDTO, PostCreateGroupReq>();
        CreateMap<GroupMember, GroupMemberDTO>();
        CreateMap<GroupMemberDTO, GroupMenberReq>();
        CreateMap<GroupMenberReq, GroupMember>();
    }
}

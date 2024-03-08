using AutoMapper;
using chat_app_service.Core.Hubs;
using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Entities;
using chat_app_service.Domain.Exceptions;
using chat_app_service.Domain.Request;
using chat_app_service.Domain.Services;
using chat_app_service.Infrastructure.Databases;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace chat_app_service.Infrastructure.Repositories;

public class MessageRepository : IMessageService
{
    private readonly AppChatDbContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly IHubContext<MessageHub> _hubContext;
    public MessageRepository(AppChatDbContext databaseContext, IMapper mapper, IHubContext<MessageHub> hubContext)
    {
        _databaseContext = databaseContext;
        _mapper = mapper;
        _hubContext = hubContext;
    }

    public async Task<MessageDTO> SendMessage(PostSendMessageReq req)
    {
        try
        {


            var message = new Message()
            {
                SenderId = req.SenderId,
                ReceiverId = req.ReceiverId,
                Content = req.Content,
                CreatedAt = DateTime.Now,
                GroupId = req.GroupId,
                Type = req.Type,

            };
            await _hubContext.Clients.All.SendAsync("ReceiveMessage", message);
            _databaseContext.Messages.Add(message);
            await _databaseContext.SaveChangesAsync();



            /// add last message
            var group = await _databaseContext.Groups.FindAsync(message.GroupId);

            if (group != null && group.LastMessage != null)
            {
                // So sánh timestamp hoặc ID để xác định tin nhắn nào mới hơn
                if (message.CreatedAt > group.LastMessage.CreatedAt)
                {
                    group.LastMessageId = message.MessageId;
                }

                await _databaseContext.SaveChangesAsync();
            }
            else
            {
                group!.LastMessageId = message.MessageId;
                await _databaseContext.SaveChangesAsync();
            }

            return _mapper.Map<MessageDTO>(message);
        }
        catch (Exception)
        {
            throw;
        }

    }
    public async Task<GroupDTO> CreateGroup(PostCreateGroupReq req)
    {
        using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
        {
            try
            {
                var group = new Group()
                {
                    Name = req.GroupName,
                    CreatedBy = req.CreatedBy,
                    CreatedAt = DateTime.Now,
                };
                _databaseContext.Groups.Add(group);
                await _databaseContext.SaveChangesAsync();

                List<GroupMember> groupMembers = new List<GroupMember>();
                if (req.Members != null && req.Members.Any())
                {
                    foreach (var groupMembersReq in req.Members)
                    {
                        var groupMember = _mapper.Map<GroupMember>(groupMembersReq);
                        groupMember.GroupId = group.GroupId;
                        groupMember.JoinedAt = group.CreatedAt;
                        groupMember.Userid = groupMembersReq.Userid ?? -1;

                        groupMembers.Add(groupMember);
                    }
                }

                group.GroupMembers = groupMembers;

                _databaseContext.GroupMembers.AddRange(groupMembers);
                await _databaseContext.SaveChangesAsync();
                await transaction.CommitAsync();

                var newGroup = await _databaseContext.Groups.AsNoTracking()
                .Where(g => g.GroupMembers.Any(gm => gm.GroupId == group.GroupId))
                .Include(g => g.LastMessage)
                .Include(g => g.GroupMembers)
                .ThenInclude(gm => gm.User).FirstOrDefaultAsync();

                return _mapper.Map<GroupDTO>(GroupToDTO(newGroup!));
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }

    public async Task<List<GroupDTO>> GetGroups(long userId)
    {
        try
        {
            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Userid == userId);
            if (user == null)
            {
                throw new NotFoundException("User does not exist");
            }

            var groups = await _databaseContext.Groups.AsNoTracking().Include(g => g.LastMessage)
           .Where(g => g.GroupMembers.Any(gm => gm.Userid == userId))
           .Include(g => g.GroupMembers)
           .ThenInclude(gm => gm.User)
           .ToListAsync();

            var listGroup = groups.Select(g => GroupToDTO(g)).ToList();

            return _mapper.Map<List<GroupDTO>>(listGroup);
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<MessageDTO>> GetMessages(long groupId)
    {
        try
        {
            if (!_databaseContext.Groups.Any(u => u.GroupId == groupId))
            {
                throw new BadHttpRequestException(message: "group Không tồn tại");
            }
            var messages = await _databaseContext.Messages.AsNoTracking()
            .Where(m => m.GroupId == groupId)
            .ToListAsync();

            if (messages.Count == 0) return new List<MessageDTO>();

            return _mapper.Map<List<MessageDTO>>(messages);
        }
        catch
        {
            throw;
        }
    }

    public async Task<List<MessageDTO>> GetMessagesBetweenUsers(long userIdA, long userIdB)
    {
        try
        {
            var messages = await _databaseContext.Messages.Where(m => ((m.SenderId == userIdA && m.ReceiverId == userIdB) ||
            (m.SenderId == userIdB && m.ReceiverId == userIdA))).ToListAsync();

            if (messages.Count == 0) return new List<MessageDTO>();

            return _mapper.Map<List<MessageDTO>>(messages);
        }
        catch
        {
            throw;
        }
    }


    private GroupDTO GroupToDTO(Group group)
    {
        return new GroupDTO
        {
            GroupId = group.GroupId,
            Name = group.Name,
            LastMessageId = group.LastMessageId,
            LastMessage = group.LastMessage != null ? new MessageDTO
            {
                MessageId = group.LastMessage.MessageId,
                SenderId = group.LastMessage.SenderId,
                ReceiverId = group.LastMessage.ReceiverId,
                GroupId = group.LastMessage.GroupId,
                Content = group.LastMessage.Content,
                Type = group.LastMessage.Type,
                CreatedAt = group.LastMessage.CreatedAt
            } : null,
            CreatedBy = group.CreatedBy,
            CreatedAt = group.CreatedAt,
            Members = group.GroupMembers.Select(m => new GroupMemberDTO
            {
                GroupId = m.GroupId,
                Userid = m.Userid,
                JoinedAt = m.JoinedAt,
                LeftAt = m.LeftAt,
                Users = new UserDTO
                {
                    Userid = m.Userid,
                    Fullname = m.User.Fullname,
                    Email = m.User.Email,
                }
            }).ToList()
        };
    }
}

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

            _databaseContext.Messages.Add(message);
            await _databaseContext.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", req);

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

                return _mapper.Map<GroupDTO>(group);
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

            // Retrieve groups and eager-load members using optimized query
            var groups = await _databaseContext.Groups
                .Where(g => g.GroupMembers.Any(gm => gm.Userid == userId))
                .Include(g => g.GroupMembers)
                .ToListAsync();

            if (groups.Count == 0) return new List<GroupDTO>();


            //  Map groups to DTOs
            var groupDTOs = groups.Select(g => new GroupDTO
            {
                GroupsId = g.GroupId,
                Name = g.Name,
                LastMessageId = g.LastMessageId,
                CreatedBy = g.CreatedBy,
                CreatedAt = g.CreatedAt,
                Menbers = g.GroupMembers.Select(gm => new GroupMenberDTO
                {
                    GroupsId = gm.GroupId,
                    Userid = gm.Userid,
                    JoinedAt = gm.JoinedAt,
                    LeftAt = gm.LeftAt
                }).ToList()
            }).ToList();

            // var groups = await _databaseContext.Groups
            //.Where(g => g.GroupMembers.Any(gm => gm.Userid == userId))
            //.Include(g => g.GroupMembers)
            //.ThenInclude(gm => gm.User)
            //.Select(g => new GroupDTO
            //{
            //    GroupsId = g.GroupId,
            //    Name = g.Name,
            //    LastMessageId = g.LastMessageId,
            //    CreatedBy = g.CreatedBy,
            //    CreatedAt = g.CreatedAt,
            //    Members = g.GroupMembers.Select(gm => new GroupMenberDTO
            //    {
            //        GroupsId = gm.GroupId,
            //        JoinedAt = gm.JoinedAt,
            //        LeftAt = gm.LeftAt,
            //        Users = new UserDTO
            //        {
            //            Userid = gm.User.Userid,
            //            Fullname = gm.User.Fullname,
            //        }
            //    }).ToList()
            //})
            //.ToListAsync();

            return _mapper.Map<List<GroupDTO>>(groupDTOs);
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
}

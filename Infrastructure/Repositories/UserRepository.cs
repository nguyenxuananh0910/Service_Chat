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
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace chat_app_service.Infrastructure.Repositories;

public class UserRepository : IUserService
{
    private readonly AppChatDbContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IHubContext<MessageHub> _hubContext;

    public UserRepository(AppChatDbContext databaseContext, IMapper mapper, IConfiguration configuration, IHubContext<MessageHub> hubContext)
    {
        _databaseContext = databaseContext;
        _mapper = mapper;
        _configuration = configuration;
        _hubContext = hubContext;
    }




    public async Task<SignDTO> Sign(PostSignReq req)
    {
        using (var transaction = await _databaseContext.Database.BeginTransactionAsync())
        {
            try
            {
                var user = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == req.Username);
                if (user != null && user.Userid > 0)
                    throw new BadHttpRequestException(message: "Tài khoản đã được đăng ký");
                else
                {
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword(req.Password);
                    var newUser = new User()
                    {
                        Username = req.Username,
                        Password = hashedPassword,
                        Email = req.Email,
                        Fullname = req.Fullname,
                        CreatedAt = DateTime.Now
                    };
                    _databaseContext.Users.Add(newUser);
                    await _databaseContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return _mapper.Map<SignDTO>(newUser);
                }
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

    public async Task<LoginDTO> Login(PostLoginReq req)
    {
        try
        {

            var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
            if (user == null)
            {
                throw new BadHttpRequestException(message: "User Không tồn tại");
            }
            if (!BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
            {
                throw new BadHttpRequestException(message: "Email hoặc password không đúng");
            }

            var token = CreateToken(req.Username ?? "");

            var userLogin = new LoginDTO()
            {
                Userid = user.Userid,
                Username = req.Username,
                Fullname = user.Fullname,
                Email = user.Email,
                token = token,
            };

            return _mapper.Map<LoginDTO>(userLogin);
        }
        catch
        {
            throw;
        }
    }
    public async Task<List<UserDTO>> GetUsers(long userId)
    {
        var user = await _databaseContext.Users.Where(u => u.Userid != userId).AsNoTracking().ToListAsync();
        if (user == null)
        {
            throw new NotFoundException("User Không tồn tại");
        }
        return _mapper.Map<List<UserDTO>>(user);
    }


    private string CreateToken(string username)
    {
        List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username),

            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;

    }

    public async Task<UserDTO> GetUser(long userId)
    {
        try
        {
            var user = await _databaseContext.Users.AsNoTracking()
          .Where(e => e.Userid == userId)
          .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NotFoundException("User Không tồn tại");
            }

            return _mapper.Map<UserDTO>(user);
        }
        catch
        {
            throw;
        }
    }

    //public async Task<string> UpdateUserStatus(long userId, bool status)
    //{
    //    //Retrieve the user from the database
    //    var user = await _databaseContext.Users.FirstOrDefaultAsync(u => u.Userid == userId);

    //    // Check if the user exists
    //    if (user != null)
    //    {
    //        // Update the user's status
    //        user.Status = status;

    //        // Save changes to the database
    //        await _databaseContext.SaveChangesAsync();
    //    }

    //    // Broadcast the updated status to all clients
    //    await _hubContext.Clients.All.SendAsync("UpdateUserStatus", userId, status);

    //    return "succ";
    //}
}

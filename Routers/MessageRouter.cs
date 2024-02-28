using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Exceptions;
using chat_app_service.Domain.Request;
using chat_app_service.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace chat_app_service.Routers;
public static class MessageRouter
{
    public static void MapMessageRouter(this IEndpointRouteBuilder app)
    {
        string tag = "Message Offices";


        app.MapPost("/Message", async (IMessageService messageService, PostSendMessageReq body) =>
        {

            var result = await messageService.SendMessage(body);

            return result == null ?
            Results.NotFound(new BaseResponse<dynamic>()
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Không thể gui tin nhan",
                error = "Không thể gui tin nhan"
            }) :
            Results.Ok(new BaseResponse<MessageDTO>()
            {
                data = result
            });
        }).WithTags(tag)
     .WithName("SendMessage")
     .Produces<BaseResponse<MessageDTO>>();

        app.MapPost("/CreateGroup", async (IMessageService messageService, PostCreateGroupReq body) =>
        {

            var result = await messageService.CreateGroup(body);

            return result == null ?
            Results.NotFound(new BaseResponse<dynamic>()
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Không thể gui tin nhan",
                error = "Không thể gui tin nhan"
            }) :
            Results.Ok(new BaseResponse<GroupDTO>()
            {
                data = result
            });
        }).WithTags(tag)
     .WithName("CreateGroup")
     .Produces<BaseResponse<GroupDTO>>();

        app.MapGet("/Groups" + "/{userId}", async ([FromServices] IMessageService messageService, long userId) =>
        {
            var results = await messageService.GetGroups(userId);

            return Results.Ok(new BaseResponse<List<GroupDTO>>
            {
                data = results
            });
        }).WithTags(tag).WithName("Groups")
      .Produces<BaseResponse<List<GroupDTO>>>();

        app.MapGet("/Messages" + "/{groupId}", async ([FromServices] IMessageService messageService, long groupId) =>
        {
            var results = await messageService.GetMessages(groupId);

            return Results.Ok(new BaseResponse<List<MessageDTO>>
            {
                data = results
            });
        }).WithTags(tag).WithName("Messages")
       .Produces<BaseResponse<List<MessageDTO>>>();


        app.MapGet("/CheckIfUsersMessaged" + "/{userAId }" + "/{userBId }", async ([FromServices] IMessageService messageService, long userAId, long userBId) =>
        {
            var results = await messageService.GetMessagesBetweenUsers(userAId, userBId);

            return Results.Ok(new BaseResponse<List<MessageDTO>>
            {
                data = results
            });
        }).WithTags(tag).WithName("CheckIfUsersMessaged")
      .Produces<BaseResponse<List<MessageDTO>>>();
    }
}

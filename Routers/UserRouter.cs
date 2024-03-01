using chat_app_service.Domain.DTOs;
using chat_app_service.Domain.Exceptions;
using chat_app_service.Domain.Request;
using chat_app_service.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace Travel_Ease.Routers;

public static class UserRouter
{
    public static void MapUserRoute(this IEndpointRouteBuilder app)
    {
        string tag = "User Offices";

        app.MapPost("/sign", async (IUserService userService, PostSignReq body) =>
        {

            var result = await userService.Sign(body);

            return result == null ?
            Results.NotFound(new BaseResponse<dynamic>()
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Không thể sign",
                error = "Không thể sign"
            }) :
            Results.Ok(new BaseResponse<SignDTO>()
            {
                data = result
            });
        }).WithTags(tag)
     .WithName("SignUser")
     .Produces<BaseResponse<SignDTO>>();

        app.MapPost("/login", async (IUserService userService, PostLoginReq body) =>
        {

            var result = await userService.Login(body);

            return result == null ?
            Results.NotFound(new BaseResponse<dynamic>()
            {
                statusCode = (int)HttpStatusCode.NotFound,
                message = "Không thể login",
                error = "Không thể login"
            }) :
            Results.Ok(new BaseResponse<LoginDTO>()
            {
                data = result
            });
        }).WithTags(tag)
     .WithName("LoginUser")
     .Produces<BaseResponse<LoginDTO>>();

        app.MapGet("/users", async ([FromServices] IUserService userService) =>
        {
            var results = await userService.GetUsers();

            return Results.Ok(new BaseResponse<List<UserDTO>>
            {
                data = results
            });
        }).WithTags(tag).WithName("GetUsers")
      .Produces<BaseResponse<List<UserDTO>>>();

        app.MapGet("/user" + "/{userId}", async ([FromServices] IUserService userService, long userId) =>
        {
            var results = await userService.GetUser(userId);

            return Results.Ok(new BaseResponse<UserDTO>
            {
                data = results
            });
        }).WithTags(tag).WithName("GetUser")
      .Produces<BaseResponse<UserDTO>>();
    }
}


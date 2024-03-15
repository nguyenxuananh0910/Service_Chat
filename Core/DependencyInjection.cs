using AutoMapper;
using chat_app_service.Core.Authentication;
using chat_app_service.Domain.Services;
using chat_app_service.Infrastructure.Repositories;
using Microsoft.AspNetCore.SignalR;



namespace chat_app_service.Core;

public static class DependencyInjection
{
    /// <summary>
    /// Register all services, repositories, etc. to DI container to use in application
    /// 
    /// Add this to ServiceCollection on startup application
    /// </summary>
    /// <param name="services"></param>

    public static void AddPersistence(this IServiceCollection services)
    {
        #region Add AutoMapper

        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MapperProfile());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        #endregion

        services.AddScoped<IUserService, UserRepository>();
        services.AddScoped<IMessageService, MessageRepository>();
        services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

        //services.AddSingleton<MessageHub>();

    }
}

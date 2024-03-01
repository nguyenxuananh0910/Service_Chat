using chat_app_service.Core;
using chat_app_service.Core.Hubs;
using chat_app_service.Domain.Exceptions;
using chat_app_service.Infrastructure.Databases;
using chat_app_service.Routers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Travel_Ease.Routers;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
    DotNetEnv.Env.LoadMulti(new[] { ".env", ".env.dev", });
else if (builder.Environment.IsStaging())
    DotNetEnv.Env.LoadMulti(new[] { ".env", ".env.stg", });
else if (builder.Environment.IsProduction())
    DotNetEnv.Env.LoadMulti(new[] { ".env", ".env.prod", });

#region Config Json Serializer
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
#endregion

#region Config Logging

builder.Logging.ClearProviders();
string _logSavePath = EnvironmentExt.getVariable<string>(EnviromentValueKeys.LogConfigs.SAVE_PATH);
string _logMinimumLevel = EnvironmentExt.getVariable<string>(EnviromentValueKeys.LogConfigs.MINIMUM_LEVEL);
string _logOutputTemplate = EnvironmentExt.getVariable<string>(EnviromentValueKeys.LogConfigs.OUTPUT_TEMPLATE);

Serilog.Events.LogEventLevel logEventLevel = Enum.Parse<Serilog.Events.LogEventLevel>(_logMinimumLevel);

var logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", logEventLevel)
    .WriteTo.File(_logSavePath, rollingInterval: RollingInterval.Day, outputTemplate: _logOutputTemplate)
    .CreateLogger();
Log.Logger = logger;
builder.Logging.AddSerilog(logger);

#endregion

#region Config Database

string _connectionString = EnvironmentExt.getVariable<string>(EnviromentValueKeys.DatabaseConfigs.CONNECTION_STRING);

builder.Services.AddDbContext<AppChatDbContext>(
    options =>
    {
        options.UseSqlServer(_connectionString
   );

    },
    contextLifetime: ServiceLifetime.Transient
);

#endregion


builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = false,
        ValidateIssuer = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value!))
    };
});


//Add all service dependencies
builder.Services.AddPersistence(); //dependency injection for persistence
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
//Remove default bad request response
builder.Services.Configure<RouteHandlerOptions>(o => o.ThrowOnBadRequest = true);




var app = builder.Build();


if (app.Environment.IsDevelopment() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
app.MapHub<MessageHub>("/messagehub");

#region Mapping Routes

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.MapHealthChecks("/");
app.MapUserRoute();
app.MapMessageRouter();
#endregion





app.Run();
using chat_app_service.Core;
using chat_app_service.Core.Hubs;
using chat_app_service.Domain.Exceptions;
using chat_app_service.Infrastructure.Databases;
using chat_app_service.Routers;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;




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
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetSection("AppSettings:Token").Value!))

    };

    options.Authority = "Authority URL"; // TODO: Update URL

    // We have to hook the OnMessageReceived event in order to
    // allow the JWT authentication handler to read the access
    // token from the query string when a WebSocket or 
    // Server-Sent Events request comes in.

    // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
    // due to a limitation in Browser APIs. We restrict it to only calls to the
    // SignalR hub in this code.
    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
    // for more information about security considerations when using
    // the query string to transmit the access token.
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];

            // If the request is for our hub...
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/message-hub")))
            {
                // Read the token out of the query string
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };

});

// FireBase cofig
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "D:\\service\\Service_Chat\\send-notification-3738e-firebase-adminsdk-i48xu-8da7ca0b2e.json")),
});

//Add all service dependencies
builder.Services.AddPersistence(); //dependency injection for persistence
builder.Services.AddHealthChecks();
builder.Services.AddCors();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
//builder.Services.AddRazorPages();


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

//app.UseCors(builder =>
//{
//    builder.WithOrigins("http://localhost:4200")
//    .AllowAnyHeader().AllowAnyMethod().AllowCredentials();
//});

#region Mapping Routes

app.UseMiddleware<CustomExceptionHandlerMiddleware>();
app.MapHealthChecks("/");
//app.UseAuthentication();
//app.UseAuthorization();
app.MapUserRoute();
app.MapMessageRouter();
//app.MapRazorPages();
//app.UseHttpsRedirection();
//app.UseStaticFiles();
// Configure the HTTP request pipeline.
app.MapHub<MessageHub>("/message-hub");

#endregion


app.Run();
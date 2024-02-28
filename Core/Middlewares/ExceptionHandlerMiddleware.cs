using chat_app_service.Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Http.Features;
using Serilog;
using Serilog.Events;
using System.Diagnostics;
using System.Net;
using System.Text.Json;


public class CustomExceptionHandlerMiddleware
{
    const string MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";

    private readonly RequestDelegate _next;
    private readonly Serilog.ILogger _logger = Log.ForContext<CustomExceptionHandlerMiddleware>();

    static readonly HashSet<string> HeaderWhitelist = new HashSet<string>
        {
            "Content-Type",
            "Content-Length",
            "User-Agent"
        };

    public CustomExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        var start = Stopwatch.GetTimestamp();
        try
        {
            await _next(httpContext);
            var elapsedMs = GetElapsedMilliseconds(start, Stopwatch.GetTimestamp());

            var statusCode = httpContext.Response?.StatusCode;
            var level = statusCode > 499 ? LogEventLevel.Error : LogEventLevel.Information;

            var log = level == LogEventLevel.Error ? LogForErrorContext(httpContext) : _logger;
            log.Write(
                level,
                MessageTemplate,
                httpContext.Request.Method,
                GetPath(httpContext),
                statusCode,
                elapsedMs
            );
        }
        catch (Exception ex)
        {
            LogException(
                httpContext,
                GetElapsedMilliseconds(start, Stopwatch.GetTimestamp()),
                ex
            );
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "Internal Error";
        string error = "Internal Error. Please contact administrator to get more information";

        switch (exception)
        {
            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = "Not Found";
                error = exception.Message ?? "Not Found";
                break;
            case ValidationException:
                statusCode = HttpStatusCode.BadRequest;
                message = "Bad Request";
                error = ((ValidationException)exception).Errors.ToString() ?? "Bad Request";
                break;
            case BadHttpRequestException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message ?? "Bad Request";
                error = "Bad Request";
                break;
            case BusinessException:
                statusCode = HttpStatusCode.UnprocessableEntity;
                message = exception.Message ?? "Unprocessable Entity";
                error = "Unprocessable Entity";
                break;
        }

        if (EnvironmentExt.IsDevelopment())
        {
            message = exception.Message ?? "Error";
        }

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        var errorResponse = new BaseResponse<dynamic>
        {
            statusCode = (int)statusCode,
            message = message,
            error = error,
            data = null,
        };
        var jsonContent = JsonSerializer.Serialize(errorResponse);

        return context.Response.WriteAsync(jsonContent);
    }

    static bool LogException(HttpContext httpContext, double elapsedMs, Exception ex)
    {
        LogForErrorContext(httpContext)
            .Error(
                ex,
                MessageTemplate,
                httpContext.Request.Method,
                GetPath(httpContext),
                500,
                elapsedMs
            );
        return false;
    }

    static Serilog.ILogger LogForErrorContext(HttpContext httpContext)
    {
        var request = httpContext.Request;

        var loggedHeaders = request.Headers
            .Where(h => HeaderWhitelist.Contains(h.Key))
            .ToDictionary(h => h.Key, h => h.Value.ToString());

        var result = Log.ForContext("RequestHeaders", loggedHeaders, destructureObjects: true)
            .ForContext("RequestHost", request.Host)
            .ForContext("RequestProtocol", request.Protocol);

        return result;
    }

    static double GetElapsedMilliseconds(long start, long stop)
    {
        return (stop - start) * 1000 / (double)Stopwatch.Frequency;
    }

    static string GetPath(HttpContext httpContext)
    {
        return httpContext.Features.Get<IHttpRequestFeature>()?.RawTarget
            ?? httpContext.Request.Path.ToString();
    }
}

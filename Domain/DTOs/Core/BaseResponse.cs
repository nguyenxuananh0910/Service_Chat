using System.Net;

namespace chat_app_service.Domain.Exceptions;

public class BaseResponse<T>
{
    public int statusCode { get; set; } = (int)HttpStatusCode.OK;
    public string message { get; set; } = "Success";
    public string? error { get; set; }
    public T? data { get; set; }
}

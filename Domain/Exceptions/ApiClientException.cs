using System.Net;
namespace chat_app_service.Domain.Exceptions;

public class ApiClientException : Exception
{
    public HttpStatusCode StatusCode { get; set; }
    public HttpResponseMessage httpResponseMessage { get; set; }

    public ApiClientException(string message, HttpStatusCode statusCode, HttpResponseMessage httpResponseMessage) : base(message)
    {
        StatusCode = statusCode;
        this.httpResponseMessage = httpResponseMessage;
    }
}

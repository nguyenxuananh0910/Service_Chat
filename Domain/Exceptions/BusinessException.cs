namespace chat_app_service.Domain.Exceptions;
/// <summary>
/// This exception is thrown when a business rule is violated.
/// </summary>
public class BusinessException : Exception
{
    public BusinessException(string message) : base(message)
    {
    }
}

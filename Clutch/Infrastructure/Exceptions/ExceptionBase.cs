namespace Clutch.Infrastructure.Exceptions;

public abstract class ExceptionBase(
    string message,
    int statusCode,
    string? errorCode
) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string? ErrorCode { get; } = errorCode;
}



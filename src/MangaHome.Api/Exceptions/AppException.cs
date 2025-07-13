namespace MangaHome.Api.Exceptions;

public class AppException : Exception
{
    public AppException(string message, int statusCode = 500) : base(message)
    {
        StatusCode = statusCode;
    }
    
    public int StatusCode { get; set; }
}
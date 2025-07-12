using MangaHome.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using System.Text.Json;

namespace MangaHome.Api.Common;

public class GlobalExceptionHandler : IExceptionHandler
{
    public GlobalExceptionHandler()
    {

    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        var message = Messages.ERR_UNEXPECTED_ERROR;
        var statusCode = (int)HttpStatusCode.InternalServerError;

        if (exception is AppException ex)
        {
            message = ex.Message;
            statusCode = ex.StatusCode;
        }

        var responseContent = new
        {
            Message = message
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await httpContext.Response.WriteAsync(
            JsonSerializer.Serialize(responseContent, options),
            cancellationToken);

        return true;
    }
}

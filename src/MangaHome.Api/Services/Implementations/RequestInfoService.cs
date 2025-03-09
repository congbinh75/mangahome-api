using MangaHome.Core.Abstractions;
using System.Security.Claims;

namespace MangaHome.Api.Services.Implementations;

public class RequestInfoService : IRequestInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<RequestInfoService> _logger;

    public RequestInfoService(IHttpContextAccessor httpContextAccessor,
        ILogger<RequestInfoService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? GetCurrentUserId()
    {
        var result = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        _logger.LogInformation("Requested service: {ServiceName}; Method: {MethodName}; Input: {Input}; Output: {Output}",
            this.GetType().Name,
            nameof(GetCurrentUserId),
            null,
            result);
        
        return result;
    }
}
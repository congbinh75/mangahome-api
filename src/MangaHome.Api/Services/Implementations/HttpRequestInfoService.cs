using System.Security.Claims;

namespace MangaHome.Api.Services.Implementations;

public class HttpRequestInfoService : IHttpRequestInfoService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<HttpRequestInfoService> _logger;

    public HttpRequestInfoService(
        IHttpContextAccessor httpContextAccessor,
        ILogger<HttpRequestInfoService> logger
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? GetCurrentUserId()
    {
        var result = _httpContextAccessor.HttpContext?.User?.Claims?.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        
        return result;
    }
}
using MangaHome.Core.Abstractions;

namespace MangaHome.Api.Services.Implementations;

public class DateTimeService : IDateTimeService
{
    private readonly ILogger<DateTimeService> _logger;

    public DateTimeService(ILogger<DateTimeService> logger)
    {
        _logger = logger;
    }

    public DateTimeOffset GetUtcNow()
    {
        var result = DateTimeOffset.UtcNow;

        _logger.LogInformation("Requested service: {ServiceName}; Method: {MethodName}; Input: {Input}; Output: {Output}",
            this.GetType().Name,
            nameof(GetUtcNow),
            null,
            result);

        return result;
    }
}
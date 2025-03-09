namespace MangaHome.Core.Abstractions;

public interface IDateTimeService
{
    public DateTimeOffset GetUtcNow();
}
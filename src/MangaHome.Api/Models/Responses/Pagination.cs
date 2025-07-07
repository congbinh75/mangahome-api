namespace MangaHome.Api.Models.Responses;

public record Pagination
{
    public int Size { get; set; }
    public int Page { get; set; }
    public int Total { get; set; }
}
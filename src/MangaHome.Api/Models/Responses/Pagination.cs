namespace MangaHome.Api.Models.Responses;

public record Pagination
{
    public int Limit { get; set; }
    public int Offset { get; set; }
    public int Total { get; set; }
}
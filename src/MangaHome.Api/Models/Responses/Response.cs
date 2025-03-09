using MangaHome.Api.Common;
using System.Text.Json.Serialization;

namespace MangaHome.Api.Models.Responses;

public record Response<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; set; } = true;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    [JsonPropertyName("pagination")]
    public Pagination? Pagination { get; set; }

    [JsonPropertyName("errors")]
    public ICollection<Error>? Errors { get; set; }
}
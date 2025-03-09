using System.Text.Json.Serialization;

namespace MangaHome.Api.Common;

public record Error
{
    [JsonPropertyName("type")]
    public required string Type { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
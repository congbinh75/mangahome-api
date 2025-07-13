using System.Text.Json.Serialization;

namespace MangaHome.Api.Common;

public record Error
{
    [JsonPropertyName("message")]
    public required string Message { get; set; }
}
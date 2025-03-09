using System.Text.Json;

namespace MangaHome.Api.Common;

public class Interaction
{
    public DateTimeOffset Time { get; set; }
    public required string Operation { get; set; }
    public object? Input { get; set; }
    public object? Output { get; set; }

    public override string ToString()
    {
        return "Time: " + Time.ToString() + "; Operation: " + Operation + "; Input: " + JsonSerializer.Serialize(Input) + "; Output: " + JsonSerializer.Serialize(Output) + ";";
    }
}
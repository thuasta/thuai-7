using System.Text.Json.Serialization;

namespace GameServer;

public record Result
{
    [JsonPropertyName("winner")]
    public required string Winner { get; init; }
}

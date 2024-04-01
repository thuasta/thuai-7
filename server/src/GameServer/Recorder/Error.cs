using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record Error
{
    [JsonPropertyName("messageType")]
    public string messageType => "ERROR";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("errorCode")]
    public int? errorCode { get; init; }

    [JsonPropertyName("message")]
    public string? message { get; init; }
}

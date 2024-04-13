using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record PlayerPrepareRecord : IRecord
{
    [JsonPropertyName("eventType")]
    public string eventType => "PLAYER_PREPARE";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("data")]
    public DataType? Data { get; init; }

    public record DataType
    {
        [JsonPropertyName("playerId")]
        public int? PlayerId { get; init; }

        [JsonPropertyName("turgetPosition")]
        public positionType? turgetPosition { get; init; }
    }

    public record positionType
    {
        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}

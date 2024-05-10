using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record PlayerPickUpRecord : IRecord
{
    [JsonPropertyName("eventType")]
    public string eventType => "PLAYER_PICK_UP";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("data")]
    public DataType? Data { get; init; }

    public record DataType
    {
        [JsonPropertyName("playerId")]
        public int? playerId { get; init; }

        [JsonPropertyName("token")]
        public string? token { get; init; }

        [JsonPropertyName("turgetSupply")]
        public string? targetSupply { get; init; }

        [JsonPropertyName("turgetPosition")]
        public positionType? targetPosition { get; init; }

        [JsonPropertyName("numb")]
        public int? numb { get; init; }
    }

    public record positionType
    {
        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}

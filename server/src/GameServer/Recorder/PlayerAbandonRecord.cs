using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record PlayerAbandonRecord : IRecord
{
    [JsonPropertyName("eventType")]
    public string eventType => "PLAYER_ABANDON";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("data")]
    public DataType? Data { get; init; }

    public record DataType
    {
        [JsonPropertyName("playerId")]
        public int? PlayerId { get; init; }

        [JsonPropertyName("numb")]
        public int? numb { get; init; }

        [JsonPropertyName("abandonedSupplies")]
        public string? abandonedSupplies { get; init; }
    }
}

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record PlayerUseMedicineRecord : IRecord {
    [JsonPropertyName("eventType")]
    public string eventType => "PLAYER_USE_MEDICINE";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("data")]
    public DataType? Data { get; init; }

    public record DataType {
         [JsonPropertyName("playerId")]
        public int? PlayerId { get; init; }

        [JsonPropertyName("medicine")]
        public string? medicine { get; init; }
    }
}
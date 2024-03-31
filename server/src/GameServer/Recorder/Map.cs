using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record Map {
     [JsonPropertyName("messageType")]
    public string messageType => "MAP";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("length")]
    public int? length { get; init; }

    [JsonPropertyName("walls")]
    public List<wallsPositionType>? walls { get; init; }


    public record wallsPositionType {
         [JsonPropertyName("playerId")]
        public int? PlayerId { get; init; }

        [JsonPropertyName("numb")]
        public int? numb { get; init; }

        [JsonPropertyName("abandonedSupplies")]
        public string? abandonedSupplies { get; init; }
    }
}
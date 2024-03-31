using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record Supplies {
     [JsonPropertyName("messageType")]
    public string messageType => "SUPPLIES";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("supplies")]
    public List<suppliesType>? supplies { get; init; }

    public record suppliesType{
        [JsonPropertyName("name")]
        public string? name { get; init; }

        [JsonPropertyName("position")]
        public positionType? position { get; init; }
    }

    public record positionType {

        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}
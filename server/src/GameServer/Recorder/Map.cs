using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record Map
{
    [JsonPropertyName("messageType")]
    public string messageType => "MAP";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("length")]
    public int? length { get; init; }

    [JsonPropertyName("walls")]
    public List<wallsPositionType>? walls { get; init; }


    public record wallsPositionType
    {
        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}

using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record SafeZone
{
    [JsonPropertyName("messageType")]
    public string messageType => "SAFE_ZONE";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("center")]
    public positionType? center { get; init; }

    [JsonPropertyName("radius")]
    public double? radius { get; init; }


    public record positionType
    {

        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}

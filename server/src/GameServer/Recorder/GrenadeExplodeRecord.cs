using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record GrenadeExplodeRecord : IRecord
{
    [JsonPropertyName("eventType")]
    public string eventType => "GRENADE_EXPLODE";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("explodePosition")]
    public positionType? ExplodePosition { get; init; }

    [JsonPropertyName("Victims")]
    public List<Victim>? Victims { get; init; }

    public record positionType
    {
        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }

    public record Victim
    {
        [JsonPropertyName("token")]
        public string? token { get; init; }

        [JsonPropertyName("hurt")]
        public float hurt { get; init; }
    }
}

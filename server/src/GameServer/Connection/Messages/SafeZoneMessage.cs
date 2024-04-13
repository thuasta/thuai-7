using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record SafeZoneMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "SAFE_ZONE";

    [JsonPropertyName("center")]
    public Center CenterOfCircle { get; init; } = new();

    [JsonPropertyName("radius")]
    public double Radius { get; init; }

    public record Center
    {
        [JsonPropertyName("x")]
        public double X { get; init; }

        [JsonPropertyName("y")]
        public double Y { get; init; }
    }
}

using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record TicksMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "TICKS";

    [JsonPropertyName("elapsedTicks")]
    public int ElapsedTicks { get; init; }
}

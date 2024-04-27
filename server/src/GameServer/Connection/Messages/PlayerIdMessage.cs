using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record PlayerIdMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PLAYER_ID";

    [JsonPropertyName("playerId")]
    public int PlayerId { get; init; }
}

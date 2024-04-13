using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record ErrorMessage : Message
{

    [JsonPropertyName("messageType")]
    public override string MessageType => "ERROR";

    [JsonPropertyName("errorCode")]
    public int ErrorCode { get; init; }

    [JsonPropertyName("message")]
    public string Message { get; init; } = "";
}

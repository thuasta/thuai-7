using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameServer.Connction;

public record Message
{
    /// <summary>
    /// The message type
    /// </summary>
    [JsonPropertyName("messageType")]
    public virtual string MessageType { get; init; } = "";

    /// <summary>
    /// Serialize the message to JSON
    /// </summary>
    [JsonIgnore]
    public string Json => JsonSerializer.Serialize((object)this);
}

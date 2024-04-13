using System.Text.Json;
using System.Text.Json.Serialization;

using Serilog;

namespace GameServer.Connection;

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
    public string Json
    {
        get
        {
            try
            {
                return JsonSerializer.Serialize((object)this);
            }
            catch (Exception ex)
            {
                Log.ForContext("Component", MessageType).Error($"Failed to serialize message: {ex.Message}");
                return "";
            }
        }
    }
}

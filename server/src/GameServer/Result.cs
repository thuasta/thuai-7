using System.Text.Json.Serialization;

namespace GameServer;

public record Result
{
    [JsonPropertyName("winner")]
    public required string Winner { get; init; }

    [JsonPropertyName("winnerPlayerId")]
    public required int WinnerPlayerId { get; init; }
}

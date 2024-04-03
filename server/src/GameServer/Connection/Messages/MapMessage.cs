using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record MapMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "MAP";

    [JsonPropertyName("length")]
    public int Length { get; init; }

    [JsonPropertyName("walls")]
    public List<Wall> Walls { get; init; } = new();

    public record Wall
    {
        [JsonPropertyName("wallPositions")]
        public WallPositions Position { get; init; } = new();
        public record WallPositions
        {
            [JsonPropertyName("x")]
            public int X { get; init; }

            [JsonPropertyName("y")]
            public int Y { get; init; }
        }
    }
}

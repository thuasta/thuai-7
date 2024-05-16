using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record GrenadesMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "GRENADES";

    [JsonPropertyName("grenades")]
    public List<Grenade> Grenades { get; init; } = new();

    public record Grenade
    {
        [JsonPropertyName("throwTick")]
        public int ThrowTick { get; init; } = new();

        [JsonPropertyName("evaluatedPosition")]
        public Position EvaluatedPosition { get; init; } = new();

        public record Position
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }
    }
}

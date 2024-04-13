using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record SuppliesMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "SUPPLIES";

    [JsonPropertyName("supplies")]
    public List<Supply> Supplies { get; init; } = new();

    public record Supply
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("position")]
        public Position PositionOfSupply { get; init; } = new();

        [JsonPropertyName("numb")]
        public int Numb { get; init; }

        public record Position
        {
            [JsonPropertyName("x")]
            public int X { get; init; }

            [JsonPropertyName("y")]
            public int Y { get; init; }
        }
    }
}

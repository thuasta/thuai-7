using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record PlayersInfoMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PLAYERS_INFO";

    [JsonPropertyName("players")]
    public List<Player> Players { get; init; } = new();

    public record Player
    {
        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("armor")]
        public string Armor { get; init; } = "";

        [JsonPropertyName("health")]
        public int Health { get; init; }

        [JsonPropertyName("speed")]
        public double Speed { get; init; }

        [JsonPropertyName("firearm")]
        public FirearmInfo Firearm { get; init; } = new();

        [JsonPropertyName("position")]
        public PositionInfo Position { get; init; } = new();

        [JsonPropertyName("inventory")]
        public List<Item> Inventory { get; init; } = new();

        public record FirearmInfo
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = "";

            [JsonPropertyName("distance")]
            public double Distance { get; init; }

        }

        public record PositionInfo
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }

        public record Item
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = "";

            [JsonPropertyName("num")]
            public int Num { get; init; }
        }
    }
}

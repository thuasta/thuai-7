using System.Text.Json;
using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record CompetitionUpdateMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType => "COMPETITION_UPDATE";

    [JsonPropertyName("info")]
    public Info Information { get; init; } = new();

    [JsonPropertyName("players")]
    public List<PlayerInfo> Players { get; init; } = new();

    [JsonPropertyName("events")]
    [JsonConverter(typeof(EventConverter))]
    public List<Event> Events { get; init; } = new();

    public record Info
    {
        [JsonPropertyName("elapsedTicks")]
        public int ElapsedTicks { get; init; }

        [JsonPropertyName("stage")]
        public string Stage { get; init; } = "";
    }

    public record PlayerInfo
    {
        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("armor")]
        public string Armor { get; init; } = "";

        [JsonPropertyName("current_armor_health")]
        public float Current_armor_health { get; init; }

        [JsonPropertyName("health")]
        public int Health { get; init; }

        [JsonPropertyName("speed")]
        public double Speed { get; init; }

        [JsonPropertyName("firearm")]
        public Firearm Weapon { get; init; } = new();

        [JsonPropertyName("firearms_pool")]
        public List<Firearm> WeaponSlot { get; init; } = new();

        [JsonPropertyName("inventory")]
        public List<Item> Invenroty { get; init; } = new();

        [JsonPropertyName("position")]
        public Position PlayerPosition { get; init; } = new();

        public record Firearm
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = "";

            [JsonPropertyName("windup")]
            public int Windup { get; init; }

            [JsonPropertyName("distance")]
            public int Distance { get; init; }
        }

        public record Item
        {
            [JsonPropertyName("name")]
            public string Name { get; init; } = "";

            [JsonPropertyName("num")]
            public int Num { get; init; }
        }

        public record Position
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }
    }

    public record Event
    {
        [JsonPropertyName("eventType")]
        public virtual string EventType { get; init; } = "";
    }

    public record PlayerAttackEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_ATTACK";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("targetPosition")]
        public TargetPosition TargetPos { get; init; } = new();

        public record TargetPosition
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }
    }

    public record PlayerSwitchArmEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_SWITCH_ARM";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("targetFirearm")]
        public string TargetFirearm { get; init; } = "";
    }

    public record PlayerPickUpEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_PICK_UP";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("targetSupply")]
        public string TargetSupply { get; init; } = "";

        [JsonPropertyName("targetPosition")]
        public TargetPosition TargetPos { get; init; } = new();

        public record TargetPosition
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }
    }

    public record PlayerUseMedicineEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_USE_MEDICINE";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("medicine")]
        public string Medicine { get; init; } = "";
    }

    public record PlayerUseGrenadeEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_USE_GRENADE";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("targetPosition")]
        public TargetPosition TargetPos { get; init; } = new();

        public record TargetPosition
        {
            [JsonPropertyName("x")]
            public double X { get; init; }

            [JsonPropertyName("y")]
            public double Y { get; init; }
        }
    }

    public record PlayerAbandonEvent : Event
    {
        [JsonPropertyName("eventType")]
        public override string EventType => "PLAYER_ABANDON";

        [JsonPropertyName("playerId")]
        public int PlayerId { get; init; }

        [JsonPropertyName("numb")]
        public int Numb { get; init; }

        [JsonPropertyName("abandonedSupplies")]
        public string AbandonedSupplies { get; init; } = "";
    }

    public class EventConverter : JsonConverter<Event>
    {
        public override Event? Read(
            ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonElement jsonObject = JsonDocument.ParseValue(ref reader).RootElement;

            string? eventType = jsonObject.GetProperty("eventType").GetString();

            return eventType switch
            {
                "PLAYER_ATTACK" => JsonSerializer.Deserialize<PlayerAttackEvent>(jsonObject.GetRawText(), options),
                "PLAYER_SWITCH_ARM" => JsonSerializer.Deserialize<PlayerSwitchArmEvent>(jsonObject.GetRawText(), options),
                "PLAYER_PICK_UP" => JsonSerializer.Deserialize<PlayerPickUpEvent>(jsonObject.GetRawText(), options),
                "PLAYER_USE_MEDICINE" => JsonSerializer.Deserialize<PlayerUseMedicineEvent>(jsonObject.GetRawText(), options),
                "PLAYER_USE_GRENADE" => JsonSerializer.Deserialize<PlayerUseGrenadeEvent>(jsonObject.GetRawText(), options),
                "PLAYER_ABANDON" => JsonSerializer.Deserialize<PlayerAbandonEvent>(jsonObject.GetRawText(), options),
                _ => throw new NotSupportedException(),
            };
        }

        public override void Write(
            Utf8JsonWriter writer, Event value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, (object)value, options);
        }
    }
}

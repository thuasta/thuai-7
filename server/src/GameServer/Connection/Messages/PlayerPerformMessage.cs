using System.Text.Json.Serialization;

namespace GameServer.Connection;

public record PerformAbandonMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_ABANDON";

    [JsonPropertyName("numb")]
    public int Numb { get; init; }

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("targetSupply")]
    public string TargetSupply { get; init; } = "";
}

public record PerformPickUpMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_PICK_UP";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("targetSupply")]
    public string TargetSupply { get; init; } = "";

    [JsonPropertyName("targetPosition")]
    public TargetPosition TargetPos { get; init; } = new();

    public record TargetPosition
    {
        [JsonPropertyName("x")]
        public int X { get; init; }

        [JsonPropertyName("y")]
        public int Y { get; init; }
    }
}

public record PerformSwitchArmMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_SWITCH_ARM";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("targetFirearm")]
    public string TargetFirearm{ get; init; } = "";
}

public record PerformUseMedicineMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_USE_MEDICINE";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("medicineName")]
    public string MedicineName { get; init; } = "";
}

public record PerformUseGrenadeMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_USE_GRENADE";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("targetPosition")]
    public TargetPosition TargetPos { get; init; } = new();

    public record TargetPosition
    {
        [JsonPropertyName("x")]
        public int X { get; init; }

        [JsonPropertyName("y")]
        public int Y { get; init; }
    }
}

public record PerformMoveMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_MOVE";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("direction")]
    public DirectionInfo Direction { get; init; } = new();

    public record DirectionInfo
    {
        [JsonPropertyName("x")]
        public int X { get; init; }

        [JsonPropertyName("y")]
        public int Y { get; init; }
    }
}

// TODO: Implement other messages

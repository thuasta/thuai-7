using System.Text.Json.Serialization;

namespace GameServer.Connection;

public abstract record PerformMessage : Message
{

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";
}

public record PerformAbandonMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_ABANDON";

    [JsonPropertyName("numb")]
    public int Numb { get; init; }

    [JsonPropertyName("targetSupply")]
    public string TargetSupply { get; init; } = "";
}

public record PerformPickUpMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_PICK_UP";

    [JsonPropertyName("num")]
    public int Num { get; init; }

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

public record PerformSwitchArmMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_SWITCH_ARM";

    [JsonPropertyName("targetFirearm")]
    public string TargetFirearm { get; init; } = "";
}

public record PerformUseMedicineMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_USE_MEDICINE";

    [JsonPropertyName("medicineName")]
    public string MedicineName { get; init; } = "";
}

public record PerformUseGrenadeMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_USE_GRENADE";

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

public record PerformMoveMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_MOVE";

    [JsonPropertyName("destination")]
    public DestinationInfo Destination { get; init; } = new();

    public record DestinationInfo
    {
        [JsonPropertyName("x")]
        public double X { get; init; }

        [JsonPropertyName("y")]
        public double Y { get; init; }
    }
}

public record PerformStopMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_STOP";

}

public record PerformAttackMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_ATTACK";

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

public record GetPlayerInfoMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "GET_PLAYER_INFO";
}

public record GetMapMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "GET_MAP";
}

public record ChooseOriginMessage : PerformMessage
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "CHOOSE_ORIGIN";

    [JsonPropertyName("originPosition")]
    public OriginPosition OriginPos { get; init; } = new();

    public record OriginPosition
    {
        [JsonPropertyName("x")]
        public double X { get; init; }

        [JsonPropertyName("y")]
        public double Y { get; init; }
    }
}

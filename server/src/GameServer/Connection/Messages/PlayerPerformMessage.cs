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

public record PerformSwitchArmMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_SWITCH_ARM";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

    [JsonPropertyName("targetFirearm")]
    public string TargetFirearm { get; init; } = "";
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
        public double X { get; init; }

        [JsonPropertyName("y")]
        public double Y { get; init; }
    }
}

public record PerformMoveMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_MOVE";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

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

public record PerformStopMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_STOP";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

}

public record PerformAttackMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "PERFORM_ATTACK";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

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

public record GetPlayerInfoMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "GET_PLAYER_INFO";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";
}

public record GetMapMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "GET_MAP";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";
}

public record ChooseOriginMessage : Message
{
    [JsonPropertyName("messageType")]
    public override string MessageType { get; init; } = "CHOOSE_ORIGIN";

    [JsonPropertyName("token")]
    public string Token { get; init; } = "";

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

using System.Text.Json.Serialization;

namespace GameServer;

public record Config
{

    [JsonPropertyName("safeZoneInitialRadius")]
    public int SafeZoneInitialRadius { get; init; } = 256;

    [JsonPropertyName("safeZoneShrinkTime")]
    public int SafeZoneShrinkTime { get; init; } = 6000;

    [JsonPropertyName("damagePerTickOutsideSafeZone")]
    public int DamagePerTickOutsideSafeZone { get; init; } = 1;

    [JsonPropertyName("logLevel")]
    public string LogLevel { get; init; } = "INFORMATION";

    [JsonPropertyName("logTarget")]
    public string LogTarget { get; init; } = "CONSOLE";

    [JsonPropertyName("mapLength")]
    public int MapLength { get; init; } = 256;

    [JsonPropertyName("playerCount")]
    public int PlayerCount { get; init; } = 2;

    [JsonPropertyName("queueTime")]
    public int QueueTime { get; init; } = 10;

    [JsonPropertyName("connectionLimitTime")]
    public int ConnectionLimitTime { get; init; } = 30;

    [JsonPropertyName("serverPort")]
    public int ServerPort { get; init; } = 14514;

    [JsonPropertyName("tokenListEnv")]
    public string TokenListEnv { get; init; } = "TOKEN_LIST";

    [JsonPropertyName("maxRunningSeconds")]
    public int MaxRunningSeconds { get; init; } = 600;
}

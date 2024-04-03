using System.Text.Json.Serialization;

namespace GameServer;

public record Config
{
    [JsonPropertyName("server_port")]
    public int ServerPort { get; init; } = 14514;

    [JsonPropertyName("level_name")]
    public string LevelName { get; init; } = "NovelCraft level";

    [JsonPropertyName("ticks_per_second")]
    public int TicksPerSecond { get; init; } = 20;

    [JsonPropertyName("save_level")]
    public bool SaveLevel { get; init; } = false;

    [JsonPropertyName("save_record")]
    public bool SaveRecord { get; init; } = true;

    [JsonPropertyName("waiting_time")]
    public decimal WaitingTime { get; init; } = 0;

    [JsonPropertyName("max_tick")]
    public int? MaxTick { get; init; } = null;

    [JsonPropertyName("log_level")]
    public string LogLevel { get; init; } = "INFORMATION";
}

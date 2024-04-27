using System.Text.Json.Serialization;

namespace GameServer;

public record Config
{
    [JsonPropertyName("server_port")]
    public int ServerPort { get; init; } = 14514;

    [JsonPropertyName("level_name")]
    public string LevelName { get; init; } = "DefaultLevel";

    [JsonPropertyName("ticks_per_second")]
    public int TicksPerSecond { get; init; } = 20;

    [JsonPropertyName("save_level")]
    public bool SaveLevel { get; init; } = false;

    [JsonPropertyName("save_record")]
    public bool SaveRecord { get; init; } = true;

    [JsonPropertyName("waiting_time")]
    public decimal WaitingTime { get; init; } = 10;

    [JsonPropertyName("max_tick")]
    public int? MaxTick { get; init; } = null;

    [JsonPropertyName("log_level")]
    public string LogLevel { get; init; } = "INFORMATION";

    [JsonPropertyName("map_width")]
    public int MapWidth { get; init; } = 256;

    [JsonPropertyName("map_height")]
    public int MapHeight { get; init; } = 256;

    [JsonPropertyName("safe_zone_max_radius")]
    public int SafeZoneMaxRadius { get; init; } = 256;

    [JsonPropertyName("safe_zone_ticks_until_disappear")]
    public int SafeZoneTicksUntilDisappear { get; init; } = 6000;

    [JsonPropertyName("damage_outside_safe_zone")]
    public int DamageOutsideSafeZone { get; init; } = 1;

    [JsonPropertyName("expected_player_num")]
    public int ExpectedPlayerNum { get; init; } = 1;

}

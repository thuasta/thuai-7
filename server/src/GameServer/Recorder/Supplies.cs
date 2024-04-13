using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

public record Supplies : IRecord
{
    [JsonPropertyName("messageType")]
    public string messageType => "SUPPLIES";

    [JsonIgnore]
    public JsonNode Json => JsonNode.Parse(JsonSerializer.Serialize(this))!;

    [JsonPropertyName("data")]
    public DataType? Data { get; init; }

    public record DataType
    {
        [JsonPropertyName("supplies")]
        public List<suppliesType>? supplies { get; init; }
    }
    public record suppliesType
    {
        [JsonPropertyName("name")]
        public string? name { get; init; }

        [JsonPropertyName("numb")]
        public int? numb { get; init; }

        [JsonPropertyName("position")]
        public positionType? position { get; init; }
    }


    public record positionType
    {

        [JsonPropertyName("x")]
        public double x { get; init; }

        [JsonPropertyName("y")]
        public double y { get; init; }
    }
}

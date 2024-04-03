using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace GameServer.Recorder;

/// <summary>
/// IRecord declares common interfaces of all records.
/// </summary>
public interface IRecord
{

    /// <summary>
    /// Gets the JSON representation of the record.
    /// </summary>
    public JsonNode Json { get; }
}

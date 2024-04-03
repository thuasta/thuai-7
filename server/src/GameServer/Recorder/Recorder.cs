using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json.Nodes;
using Serilog;

namespace GameServer.Recorder;

public class Recorder : IRecorder, IDisposable
{
    public const int MaxRecordsBeforeSave = 10000;

    public JsonNode Json
    {
        get
        {
            JsonObject recordJson = new()
            {
                ["type"] = "record"
            };

            JsonArray records = new();
            foreach (IRecord rec in _recordQueue.ToArray())
            {
                records.Add(rec.Json);
            }

            recordJson["records"] = records;

            return recordJson;
        }
    }

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "Recorder");
    private readonly string _recordsDir;
    private readonly ConcurrentQueue<IRecord> _recordQueue = new();
    private readonly object _saveLock = new();


    #region Constructors and finalizers
    /// <summary>
    /// Create a new recorder.
    /// </summary>
    public Recorder(string recordsDir)
    {
        _recordsDir = recordsDir;

        // Remove directory of record files if it exists.
        if (Directory.Exists(_recordsDir))
        {
            Directory.Delete(_recordsDir, true);
        }
    }
    #endregion


    #region Methods
    public void Dispose()
    {
        Save();
    }

    public void Record(IRecord record)
    {
        // Record should not be null
        if (record is null)
        {
            _logger.Error("Null record passed to Recorder.Record().");
            return;
        }

        _recordQueue.Enqueue(record);

        if (_recordQueue.Count >= MaxRecordsBeforeSave)
        {
            Save();
        }
    }

    public void Save()
    {
        if (Monitor.TryEnter(_saveLock))
        {
            try
            {
                lock (_saveLock)
                {
                    if (_recordQueue.Count == 0)
                    {
                        return;
                    }

                    JsonNode recordJson = Json;

                    _recordQueue.Clear();

                    // Use timestamp as file name.
                    long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
                    string recordFilePath = Path.Combine(_recordsDir, $"{timestamp}.dat");

                    // Create directory if it doesn't exist.
                    if (!Directory.Exists(_recordsDir))
                    {
                        Directory.CreateDirectory(_recordsDir);
                    }

                    // Write records to file.
                    using FileStream zipFile = new(recordFilePath, FileMode.CreateNew);
                    using ZipArchive archive = new(zipFile, ZipArchiveMode.Create);
                    ZipArchiveEntry entry = archive.CreateEntry("record.json", CompressionLevel.SmallestSize);
                    using StreamWriter writer = new(entry.Open());
                    writer.Write(recordJson.ToString());
                }

            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save records: {ex.Message}");

            }
            finally
            {
                Monitor.Exit(_saveLock);
            }

        }
        else
        {
            // If the lock is not acquired, it means that another thread is saving the records.
            // In this case, we just return.
            return;
        }
    }
    #endregion
}

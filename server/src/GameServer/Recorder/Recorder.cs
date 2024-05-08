using System.Collections.Concurrent;
using System.IO.Compression;
using System.Text.Json;
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
    private readonly string _targetRecordFileName;
    private readonly string _targetResultFileName;
    private readonly string _copyRecordDir;

    private readonly ConcurrentQueue<IRecord> _recordQueue = new();
    private readonly object _saveLock = new();

    #region Constructors and finalizers
    /// <summary>
    /// Create a new recorder.
    /// </summary>
    public Recorder(string recordsDir, string targetRecordFileName, string targetResultFileName)
    {
        _recordsDir = recordsDir;
        _targetRecordFileName = targetRecordFileName;
        _targetResultFileName = targetResultFileName;
        _copyRecordDir = Path.Combine(_recordsDir, "copy", $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}-{Guid.NewGuid()}");

        // Remove record file if it exists.
        if (File.Exists(Path.Combine(_recordsDir, _targetRecordFileName)))
        {
            File.Delete(Path.Combine(_recordsDir, _targetRecordFileName));
        }
        if (File.Exists(Path.Combine(_recordsDir, _targetResultFileName)))
        {
            File.Delete(Path.Combine(_recordsDir, _targetResultFileName));
        }

        if (!Directory.Exists(_recordsDir))
        {
            Directory.CreateDirectory(_recordsDir);
        }
        if (!Directory.Exists(_copyRecordDir))
        {
            Directory.CreateDirectory(_copyRecordDir);
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

        _logger.Debug($"Adding record {record.GetType().Name} to the queue.");
        _logger.Verbose($"Record: {record.Json}");

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

                    string recordFilePath = Path.Combine(_recordsDir, _targetRecordFileName);

                    long timestamp = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds * 1000;
                    string pageName = $"{timestamp}-{Guid.NewGuid()}.json";

                    _logger.Debug($"Adding new record {pageName} to {_targetRecordFileName}.");

                    // Create directory if it doesn't exist.
                    if (!Directory.Exists(_recordsDir))
                    {
                        Directory.CreateDirectory(_recordsDir);
                    }

                    // Write records to file.
                    using FileStream zipFile = new(recordFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                    using ZipArchive archive = new(zipFile, ZipArchiveMode.Update);
                    ZipArchiveEntry entry = archive.CreateEntry(pageName, CompressionLevel.SmallestSize);
                    using StreamWriter writer = new(entry.Open());
                    writer.Write(recordJson.ToString());
                    writer.Close();

                    // Create a copy of the record file.
                    using FileStream copyZipFile
                        = new(Path.Combine(_copyRecordDir, $"{timestamp}.dat"), FileMode.Create);
                    using ZipArchive copyArchive = new(copyZipFile, ZipArchiveMode.Create);
                    ZipArchiveEntry copyEntry
                        = copyArchive.CreateEntry("record.json", CompressionLevel.SmallestSize);
                    using StreamWriter copyWriter = new(copyEntry.Open());
                    copyWriter.Write(recordJson.ToString());
                    copyWriter.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save records: {ex.Message}");
                _logger.Debug($"{ex}");
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

    public void SaveResults(Result result)
    {
        string resultFilePath = Path.Combine(_recordsDir, _targetResultFileName);
        File.WriteAllText(
            resultFilePath,
            JsonSerializer.Serialize(
                result,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                }
            )
        );
    }
    #endregion
}

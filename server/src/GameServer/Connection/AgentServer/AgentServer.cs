using System.Collections.Concurrent;
using System.Text;
using Fleck;
using Serilog;

namespace GameServer.Connection;

public partial class AgentServer
{
    public const int MAXIMUM_MESSAGE_QUEUE_SIZE = 11;

    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent = delegate { };

    public string IpAddress { get; init; } = "0.0.0.0";
    public int Port { get; init; } = 8080;

    public bool WhiteListMode { get; init; } = false;
    public List<string> WhiteList { get; init; } = new();

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "AgentServer");

    private bool _isRunning = false;
    private IWebSocketServer? _wsServer = null;

    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();
    private readonly ConcurrentDictionary<Guid, string> _socketTokens = new();

    public void Start()
    {
        if (_isRunning)
        {
            _logger.Error("Cannot start AgentServer: AgentServer is already running.");
            return;
        }

        _logger.Information("Starting...");

        try
        {
            _wsServer = new WebSocketServer($"ws://{IpAddress}:{Port}");
            StartWsServer();

            _isRunning = true;

            _logger.Information("AgentServer started. Waiting for connections...");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to start AgentServer: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            _logger.Error("Cannot stop AgentServer: AgentServer is not running.");
            return;
        }

        _logger.Information("Stopping...");

        try
        {

            foreach (KeyValuePair<Guid, Task> kvp in _tasksForParsingMessage)
            {
                kvp.Value?.Dispose();
            }
            _tasksForParsingMessage.Clear();

            foreach (KeyValuePair<Guid, Task> kvp in _tasksForSendingMessage)
            {
                kvp.Value?.Dispose();
            }
            _tasksForSendingMessage.Clear();

            _wsServer?.Dispose();
            _wsServer = null;

            _isRunning = false;

            _sockets.Clear();

            _logger.Information("Stopped.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to stop AgentServer: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    /// <summary>
    /// Start the WebSocket server
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    private void StartWsServer()
    {
        if (_wsServer is null)
        {
            throw new InvalidOperationException("WebSocket server is not initialized.");
        }

        _wsServer.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                _logger.Information($"Connection from {GetAddress(socket)} opened.");

                // Remove the socket if it already exists.
                RemoveSocket(socket.ConnectionInfo.Id);
                // Add the socket.
                AddSocket(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Debug($"Connection from {GetAddress(socket)} closed.");

                // Remove the socket.
                RemoveSocket(socket.ConnectionInfo.Id);
            };

            socket.OnMessage = text =>
            {
                try
                {
                    if (_socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Count
                         > MAXIMUM_MESSAGE_QUEUE_SIZE)
                    {
                        _logger.Warning(
                            $"Received too many messages from {GetAddress(socket)}. Messages in queue will be cleared."
                        );
                        _socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Clear();
                    }

                    _socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Enqueue(text);
                    _logger.Debug($"Received text message from {GetAddress(socket)}.");
                    _logger.Verbose(text.Length > 65536 ? string.Concat(text.AsSpan(0, 65536), "...") : text);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to receive message from {GetAddress(socket)}: {exception.Message}");
                    _logger.Debug($"{exception}");
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    if (_socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Count
                         > MAXIMUM_MESSAGE_QUEUE_SIZE)
                    {
                        _logger.Warning(
                            $"Received too many messages from {GetAddress(socket)}. Messages in queue will be cleared."
                        );
                        _socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Clear();
                    }

                    string text = Encoding.UTF8.GetString(bytes);
                    _socketRawTextReceivingQueue[socket.ConnectionInfo.Id].Enqueue(text);
                    _logger.Debug(
                        $"Received binary message from {GetAddress(socket)}."
                    );
                    _logger.Verbose(text.Length > 65536 ? string.Concat(text.AsSpan(0, 65536), "...") : text);
                }
                catch (Exception exception)
                {
                    _logger.Error(
                        $"Failed to receive message from {GetAddress(socket)}: {exception.Message}"
                    );
                    _logger.Debug($"{exception}");
                }
            };

            socket.OnError = exception =>
            {
                _logger.Error($"[{GetAddress(socket)}] Socket error: {exception.Message}");

                // Close and remove the socket.
                socket.Close();
                RemoveSocket(socket.ConnectionInfo.Id);
            };
        });
    }
}

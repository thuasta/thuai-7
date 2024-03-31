using Serilog;
using Fleck;
using System.Collections.Concurrent;
using System.Text;

namespace GameServer.Connction;

public partial class AgentServer : IServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    public string IpAddress { get; } = "0.0.0.0";
    public int Port { get; } = 8080;
    public Task? TaskForPublishingMessage { get; private set; } = null;

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "AgentServer");

    /// <summary>
    /// Message to publish to clients
    /// </summary>
    private Message? _messageToPublish => throw new NotImplementedException();
    private bool _isRunning = false;
    private IWebSocketServer? _wsServer = null;
    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();

    public void Start()
    {
        if (_isRunning)
        {
            throw new InvalidOperationException("AgentServer is already running");
        }

        _logger.Information("Starting...");

        try
        {
            _wsServer = new WebSocketServer($"ws://{IpAddress}:{Port}");
            StartWsServer();

            _isRunning = true;

            TaskForPublishingMessage = Task.Run(() =>
            {
                while (_isRunning)
                {
                    Task.Delay((int)(1000 / IServer.MessagesPublishedPerSecond)).Wait();

                    if (_messageToPublish is not null)
                    {
                        Publish(_messageToPublish);
                    }
                }
            });

            _logger.Information("AgentServer started.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to start AgentServer: {ex.Message}");
        }
    }

    public void Stop()
    {
        if (!_isRunning)
        {
            throw new InvalidOperationException("AgentServer is not running");
        }

        _logger.Information("Stopping...");

        try
        {
            TaskForPublishingMessage?.Dispose();
            TaskForPublishingMessage = null;

            _wsServer?.Dispose();
            _wsServer = null;

            _isRunning = false;

            _logger.Information("Stopped.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to stop AgentServer: {ex.Message}");
        }
    }

    public void Publish(Message message)
    {
        string jsonString = message.Json;

        foreach (IWebSocketConnection socket in _sockets.Values)
        {
            try
            {
                socket.Send(jsonString).Wait();
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to send message to {socket.ConnectionInfo.ClientIpAddress}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Parse the message
    /// </summary>
    /// <param name="text">Message to parse</param>
    private void ParseMessage(string text) => throw new NotImplementedException();

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
                _logger.Debug("Connection from {ClientIpAddress} opened.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket if it already exists.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);

                // Add the socket.
                _sockets.TryAdd(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Debug("Connection from {ClientIpAddress} closed.", socket.ConnectionInfo.ClientIpAddress);

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };

            socket.OnMessage = text =>
            {
                try
                {
                    ParseMessage(text);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception}");
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    ParseMessage(text);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception}");
                }
            };

            socket.OnError = exception =>
            {
                _logger.Error("Socket error.");

                // Close the socket.
                socket.Close();

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };
        });
    }
}

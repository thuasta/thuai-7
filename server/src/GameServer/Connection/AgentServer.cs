using System.Collections.Concurrent;
using System.Text;
using Fleck;
using Serilog;

namespace GameServer.Connection;

public partial class AgentServer
{
    public const int MAXIMUM_MESSAGE_QUEUE_SIZE = 11;
    public const int TIMEOUT_MILLISEC = 10;

    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent = delegate { };

    public string IpAddress { get; init; } = "0.0.0.0";
    public int Port { get; init; } = 8080;

    public bool WhiteListMode { get; init; } = false;
    public List<string> WhiteList { get; init; } = new();


    public TimeSpan MppsCheckInterval => TimeSpan.FromSeconds(10);
    public double RealMpps { get; private set; }

    private DateTime _lastMppsCheckTime = DateTime.UtcNow;

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "AgentServer");


    private bool _isRunning = false;
    private IWebSocketServer? _wsServer = null;
    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();
    private readonly ConcurrentDictionary<Guid, string> _socketTokens = new();

    /// <summary>
    /// Message to publish to clients
    /// </summary>
    private readonly ConcurrentQueue<Message> _messageToPublish = new();
    private Task? _taskForPublishingMessage = null;

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

            Action actionForPublishingMessage = new(() =>
            {
                DateTime lastPublishTime = DateTime.UtcNow;

                while (_isRunning)
                {
                    if (_messageToPublish.Count > MAXIMUM_MESSAGE_QUEUE_SIZE)
                    {
                        _logger.Warning("Message queue is full. The messages in queue will be cleared.");
                        _messageToPublish.Clear();
                    }

                    if (_messageToPublish.IsEmpty == false && _messageToPublish.TryDequeue(out Message? message))
                    {
                        if (message is null)
                        {
                            _logger.Warning("A null message is dequeued. This message will be ignored.");
                            continue;
                        }

                        _logger.Debug($"Dequeued message \"{message.MessageType}\".");
                        _logger.Verbose(message.Json);

                        Publish(message);
                    }
                    else
                    {
                        Task.Delay(10).Wait();
                    }

                    DateTime currentTime = DateTime.UtcNow;
                    RealMpps = 1.0D / (double)(currentTime - lastPublishTime).TotalSeconds;
                    lastPublishTime = currentTime;

                    // Check MessagePublishedPerSecond.
                    if (DateTime.UtcNow - _lastMppsCheckTime > MppsCheckInterval)
                    {
                        _lastMppsCheckTime = DateTime.UtcNow;
                        _logger.Debug($"Current MessagePublishedPerSsecond: {RealMpps:0.00} msg/s");
                    }
                }
            });

            _taskForPublishingMessage = Task.Run(actionForPublishingMessage);

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
            _taskForPublishingMessage?.Dispose();
            _taskForPublishingMessage = null;

            _wsServer?.Dispose();
            _wsServer = null;

            _isRunning = false;

            _sockets.Clear();

            _messageToPublish.Clear();

            _logger.Information("Stopped.");
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to stop AgentServer: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    public void Publish(Message message, string? token = null)
    {
        try
        {
            string jsonString = message.Json;

            List<Task> sendTasks = new();

            foreach (Guid connectionId in _sockets.Keys)
            {
                try
                {
                    if (token is null || (_socketTokens.TryGetValue(connectionId, out string? val) && val == token))
                    {
                        Task task = _sockets[connectionId].Send(jsonString);
                        sendTasks.Add(task);
                        _logger.Debug($"Task {task.Id} created to send message to socket {connectionId}.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to create task to send message to socket {connectionId}: {ex.Message}");
                    _logger.Debug($"{ex}");
                }
            }

            DateTime startTime = DateTime.UtcNow;
            Task.Delay(TIMEOUT_MILLISEC).Wait();

            foreach (Task task in sendTasks)
            {
                if (task.IsCompleted == false)
                {
                    _logger.Debug($"Timeout (Task {task.Id}).");
                    continue;
                }
            }

            _logger.Debug($"Message \"{message.MessageType}\" published");
            _logger.Verbose(jsonString);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to publish message: {ex.Message}");
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
                _logger.Debug(
                    $"Connection from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort} opened."
                );

                // Remove the socket if it already exists.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);

                // Add the socket.
                _sockets.TryAdd(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Debug(
                    $"Connection from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort} closed."
                );

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
                _socketTokens.TryRemove(socket.ConnectionInfo.Id, out _);
            };

            socket.OnMessage = text =>
            {
                _logger.Debug(
                    $"Received text message from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort}."
                );

                try
                {
                    ParseMessage(text, socket.ConnectionInfo.Id);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception.Message}");
                    _logger.Debug($"{exception}");
                }
            };

            socket.OnBinary = bytes =>
            {
                _logger.Debug(
                    $"Received binary message from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort}."
                );

                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    ParseMessage(text, socket.ConnectionInfo.Id);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception.Message}");
                    _logger.Debug($"{exception}");
                }
            };

            socket.OnError = exception =>
            {
                _logger.Error($"Socket error: {exception.Message}");

                // Close and remove the socket.
                socket.Close();
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
                _socketTokens.TryRemove(socket.ConnectionInfo.Id, out _);
            };
        });
    }
}

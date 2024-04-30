using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Fleck;
using Serilog;

namespace GameServer.Connection;

public partial class AgentServer
{
    // In milliseconds.
    public const int MESSAGE_PUBLISHED_PER_SECOND = 100;
    public const int MAXIMUM_MESSAGE_QUEUE_SIZE = 11;
    public const int TIMEOUT_MILLISEC = 10;

    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent = delegate { };

    public string IpAddress { get; init; } = "0.0.0.0";
    public int Port { get; init; } = 8080;

    public List<string> WhiteList { get; init; } = new();

    public Task? TaskForPublishingMessage { get; private set; } = null;

    public TimeSpan MppsCheckInterval => TimeSpan.FromSeconds(10);
    public double RealMpps { get; private set; }
    public double MppsLowerBound => 0.9 * MESSAGE_PUBLISHED_PER_SECOND;

    private DateTime _lastMppsCheckTime = DateTime.Now;

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "AgentServer");

    /// <summary>
    /// Message to publish to clients
    /// </summary>
    private readonly ConcurrentQueue<Message> _messageToPublish = new();
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

            Action actionForPublishingMessage = new(() =>
            {
                DateTime lastPublishTime = DateTime.Now;

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

                    DateTime currentTime = DateTime.Now;
                    RealMpps = 1.0D / (double)(currentTime - lastPublishTime).TotalSeconds;
                    lastPublishTime = currentTime;

                    // Check TPS.
                    if (DateTime.Now - _lastMppsCheckTime > MppsCheckInterval)
                    {
                        _lastMppsCheckTime = DateTime.Now;

                        _logger.Debug($"Current message published per second: {RealMpps:0.00} msg/s");

                        if (RealMpps < MppsLowerBound)
                        {
                            _logger.Warning($"Insufficient publish rate: {RealMpps:0.00} msg/s < {MppsLowerBound} msg/s");
                        }
                    }
                }
            });

            TaskForPublishingMessage = Task.Run(actionForPublishingMessage);

            _logger.Information("AgentServer started. Waiting for connections...");
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
            _logger.Error("Cannot stop AgentServer: AgentServer is not running.");
            return;
        }

        _logger.Information("Stopping...");

        try
        {
            TaskForPublishingMessage?.Dispose();
            TaskForPublishingMessage = null;

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
        }
    }

    public void Publish(Message message, string? token = null)
    {
        string jsonString = message.Json;

        foreach (Guid connectionId in _sockets.Keys)
        {
            try
            {
                if (token is null || _socketTokens[connectionId] == token)
                {
                    Task sendTask = _sockets[connectionId].Send(jsonString);

                    TimeSpan timeout = TimeSpan.FromMilliseconds(TIMEOUT_MILLISEC);
                    DateTime startTime = DateTime.Now;
                    while (DateTime.Now - startTime < timeout && sendTask.IsCompleted == false)
                    {
                        // Wait until the task is completed or the timeout is reached.
                    }

                    if (sendTask.IsCompletedSuccessfully == false)
                    {
                        _logger.Error(
                            $"Failed to send message to \"{_sockets[connectionId].ConnectionInfo.ClientIpAddress}: {_sockets[connectionId].ConnectionInfo.ClientPort}\"."
                            );
                        continue;
                    }

                    _logger.Debug($"Message published");
                    _logger.Debug(
                        $"Target ip: \"{_sockets[connectionId].ConnectionInfo.ClientIpAddress}\""
                    );
                    _logger.Debug(
                        $"Target port: \"{_sockets[connectionId].ConnectionInfo.ClientPort}\""
                    );
                    _logger.Debug(
                        $"Message type: \"{message.MessageType}\""
                    );
                    _logger.Verbose(jsonString);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(
                    $"Failed to send message to \"{_sockets[connectionId].ConnectionInfo.ClientIpAddress}: {_sockets[connectionId].ConnectionInfo.ClientPort}\": {ex.Message}"
                );
            }
        }
    }

    /// <summary>
    /// Parse the message
    /// </summary>
    /// <param name="text">Message to parse</param>
    private void ParseMessage(string text, Guid socketId)
    {
        try
        {
            Message? message = JsonSerializer.Deserialize<Message>(text)
                               ?? throw new Exception("failed to deserialize Message");

            _logger.Debug("Received message: {MessageType}", message.MessageType);
            _logger.Verbose(text);

            switch (message.MessageType)
            {
                case "PERFORM_ABANDON":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformAbandonMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformAbandonMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_PICK_UP":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformPickUpMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformPickUpMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_SWITCH_ARM":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformSwitchArmMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformSwitchArmMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_USE_MEDICINE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformUseMedicineMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformUseMedicineMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_USE_GRENADE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformUseGrenadeMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformUseGrenadeMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_MOVE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformMoveMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformMoveMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_STOP":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformStopMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformStopMessage"),
                        socketId
                    ));
                    break;

                case "PERFORM_ATTACK":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformAttackMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformAttackMessage"),
                        socketId
                    ));
                    break;

                case "GET_PLAYER_INFO":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<GetPlayerInfoMessage>(text)
                        ?? throw new Exception("failed to deserialize GetPlayerInfoMessage"),
                        socketId
                    ));
                    break;

                case "GET_MAP_INFO":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<GetMapMessage>(text)
                        ?? throw new Exception("failed to deserialize GetMapInfoMessage"),
                        socketId
                    ));
                    break;

                case "CHOOSE_ORIGIN":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<ChooseOriginMessage>(text)
                        ?? throw new Exception("failed to deserialize ChooseOriginMessage"),
                        socketId
                    ));
                    break;

                default:
                    throw new InvalidOperationException($"Invalid message type {message.MessageType}.");
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to parse message: {exception.Message}");
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
                _logger.Information(
                    $"Connection from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort} opened."
                );

                // Remove the socket if it already exists.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);

                // Add the socket.
                _sockets.TryAdd(socket.ConnectionInfo.Id, socket);
            };

            socket.OnClose = () =>
            {
                _logger.Information(
                    $"Connection from {socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort} closed."
                );

                // Remove the socket.
                _sockets.TryRemove(socket.ConnectionInfo.Id, out _);
            };

            socket.OnMessage = text =>
            {
                try
                {
                    ParseMessage(text, socket.ConnectionInfo.Id);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception.Message}");
                }
            };

            socket.OnBinary = bytes =>
            {
                try
                {
                    string text = Encoding.UTF8.GetString(bytes);
                    ParseMessage(text, socket.ConnectionInfo.Id);
                }
                catch (Exception exception)
                {
                    _logger.Error($"Failed to parse message: {exception.Message}");
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

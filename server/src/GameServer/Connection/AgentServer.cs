using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using Fleck;
using Serilog;

namespace GameServer.Connection;

public partial class AgentServer : IServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent = delegate { };

    public string IpAddress { get; init; } = "0.0.0.0";
    public int Port { get; init; } = 8080;
    public Task? TaskForPublishingMessage { get; private set; } = null;

    private readonly ILogger _logger = Log.Logger.ForContext("Component", "AgentServer");

    /// <summary>
    /// Message to publish to clients
    /// </summary>
    private readonly ConcurrentQueue<Message> _messageToPublish = new();
    private bool _isRunning = false;
    private IWebSocketServer? _wsServer = null;
    private readonly ConcurrentDictionary<Guid, IWebSocketConnection> _sockets = new();

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
                while (_isRunning)
                {
                    Task.Delay(IServer.MessagePublishIntervalMilliseconds).Wait();

                    if (_messageToPublish.IsEmpty == false && _messageToPublish.TryDequeue(out Message? message))
                    {
                        if (message is null)
                        {
                            _logger.Warning("A null message is dequeued. This message will be ignored.");
                            continue;
                        }
                        Publish(message);
                    }
                }
            });

            TaskForPublishingMessage = Task.Run(actionForPublishingMessage);

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

    public void Publish(Message message)
    {
        string jsonString = message.Json;

        foreach (IWebSocketConnection socket in _sockets.Values)
        {
            try
            {
                socket.Send(jsonString).Wait();

                _logger.Debug("Published message: {MessageType}", message.MessageType);
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
    private void ParseMessage(string text)
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
                        ?? throw new Exception("failed to deserialize PerformAbandonMessage")
                    ));
                    break;

                case "PERFORM_PICK_UP":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformPickUpMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformPickUpMessage")
                    ));
                    break;

                case "PERFORM_SWITCH_ARM":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformSwitchArmMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformSwitchArmMessage")
                    ));
                    break;

                case "PERFORM_USE_MEDICINE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformUseMedicineMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformUseMedicineMessage")
                    ));
                    break;

                case "PERFORM_USE_GRENADE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformUseGrenadeMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformUseGrenadeMessage")
                    ));
                    break;

                case "PERFORM_MOVE":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformMoveMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformMoveMessage")
                    ));
                    break;

                case "PERFORM_STOP":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformStopMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformStopMessage")
                    ));
                    break;

                case "PERFORM_ATTACK":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<PerformAttackMessage>(text)
                        ?? throw new Exception("failed to deserialize PerformAttackMessage")
                    ));
                    break;

                case "GET_PLAYER_INFO":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<GetPlayerInfoMessage>(text)
                        ?? throw new Exception("failed to deserialize GetPlayerInfoMessage")
                    ));
                    break;

                case "GET_MAP_INFO":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<GetMapMessage>(text)
                        ?? throw new Exception("failed to deserialize GetMapInfoMessage")
                    ));
                    break;

                case "CHOOSE_ORIGIN":
                    AfterMessageReceiveEvent?.Invoke(this, new AfterMessageReceiveEventArgs(
                        JsonSerializer.Deserialize<ChooseOriginMessage>(text)
                        ?? throw new Exception("failed to deserialize ChooseOriginMessage")
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
                    _logger.Error($"Failed to parse message: {exception.Message}");
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

using System.Collections.Concurrent;
using System.Text.Json;

namespace GameServer.Connection;

public partial class AgentServer
{
    public const int MESSAGE_PARSE_INTERVAL = 10;

    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<string>> _socketRawTextReceivingQueue = new();
    private readonly ConcurrentDictionary<Guid, Task> _tasksForParsingMessage = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _ctsForParsingMessage = new();

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

            _logger.Debug(
                $"Parsing message: \"{(message.MessageType.Length > 32 ? string.Concat(message.MessageType.AsSpan(0, 32), "...") : message.MessageType)}\""
            );
            _logger.Verbose(text.Length > 65536 ? string.Concat(text.AsSpan(0, 65536), "...") : text);

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
                    throw new InvalidOperationException(
                        $"Invalid message type {(message.MessageType.Length > 32 ? string.Concat(message.MessageType.AsSpan(0, 32), "...") : message.MessageType)}."
                    );
            }
        }
        catch (Exception exception)
        {
            _logger.Error($"Failed to parse message: {exception.Message}");
            _logger.Debug($"{exception}");
        }
    }

    private Task CreateTaskForParsingMessage(Guid socketId)
    {
        _logger.Debug($"Creating task for parsing message from {GetAddress(socketId)}...");

        CancellationTokenSource cts = new();
        _ctsForParsingMessage.AddOrUpdate(
            socketId,
            cts,
            (key, oldValue) =>
            {
                oldValue?.Cancel();
                return cts;
            }
        );

        return new(() =>
        {
            while (_isRunning)
            {
                if (cts.IsCancellationRequested == true)
                {
                    _logger.Debug($"Request task for parsing message from {GetAddress(socketId)} to be cancelled.");
                    return;
                }

                try
                {
                    if (_socketRawTextReceivingQueue.TryGetValue(socketId, out ConcurrentQueue<string>? queue))
                    {
                        if (queue.TryDequeue(out string? text) && text is not null)
                        {
                            ParseMessage(text, socketId);
                        }
                        else
                        {
                            Task.Delay(MESSAGE_PARSE_INTERVAL).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to parse message from {GetAddress(socketId)}: {ex.Message}");
                    _logger.Debug($"{ex}");
                }
            }
        }, cts.Token);
    }
}

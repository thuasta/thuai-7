using System.Collections.Concurrent;

namespace GameServer.Connection;

public partial class AgentServer
{
    public const int MESSAGE_SENDING_INTERVAL = 10;

    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Message>> _socketMessageSendingQueue = new();
    private readonly ConcurrentDictionary<Guid, Task> _tasksForSendingMessage = new();
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _ctsForSendingMessage = new();

    public void Publish(Message message, string? token = null)
    {
        try
        {
            foreach (Guid connectionId in _sockets.Keys)
            {
                try
                {
                    if (token is null || (_socketTokens.TryGetValue(connectionId, out string? val) && val == token))
                    {
                        if (_socketMessageSendingQueue.TryGetValue(
                            connectionId, out ConcurrentQueue<Message>? queue
                            ) && queue is not null)
                        {
                            queue.Enqueue(message);
                        }
                        else
                        {
                            _socketMessageSendingQueue.AddOrUpdate(
                                connectionId,
                                new ConcurrentQueue<Message>(),
                                (key, oldValue) => new ConcurrentQueue<Message>()
                            );
                            _socketMessageSendingQueue[connectionId].Enqueue(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to add message to message queue of socket {connectionId}: {ex.Message}");
                    _logger.Debug($"{ex}");
                }
            }

            _logger.Debug($"Message \"{message.MessageType}\" published{(token is null ? "" : (" to " + token))}.");
            _logger.Verbose(message.Json);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to publish message: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private Task CreateTaskForSendingMessage(Guid socketId)
    {
        _logger.Debug($"Creating task for sending message to {GetAddress(socketId)}...");

        CancellationTokenSource cts = new();
        _ctsForSendingMessage.AddOrUpdate(
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
                    _logger.Debug($"Request task for sending message to {GetAddress(socketId)} to be cancelled.");
                    return;
                }

                try
                {
                    if (_socketMessageSendingQueue.TryGetValue(socketId, out ConcurrentQueue<Message>? queue))
                    {
                        if (queue.Count > MAXIMUM_MESSAGE_QUEUE_SIZE)
                        {
                            _logger.Warning($"Message queue for sending to {GetAddress(socketId)} is full. The messages in queue will be cleared.");
                            queue.Clear();
                        }

                        if (queue.TryDequeue(out Message? message) && message is not null)
                        {
                            _sockets[socketId].Send(message.Json);
                            _logger.Debug($"Sent message \"{message.MessageType}\" to {GetAddress(socketId)}.");
                        }
                        else
                        {
                            Task.Delay(MESSAGE_SENDING_INTERVAL).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to send message to {GetAddress(socketId)}: {ex.Message}");
                    _logger.Debug($"{ex}");
                }
            }
        }, cts.Token);
    }
}

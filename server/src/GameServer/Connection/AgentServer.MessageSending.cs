using System.Collections.Concurrent;

namespace GameServer.Connection;

public partial class AgentServer
{
    public const int MESSAGE_SENDING_INTERVAL = 10;

    private readonly ConcurrentDictionary<Guid, ConcurrentQueue<Message>> _socketMessageSendingQueue = new();
    private readonly ConcurrentDictionary<Guid, Task> _tasksForSendingMessage = new();

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
        return new(() =>
        {
            while (_isRunning)
            {
                try
                {
                    if (_socketMessageSendingQueue.TryGetValue(socketId, out ConcurrentQueue<Message>? queue))
                    {
                        if (queue.Count > MAXIMUM_MESSAGE_QUEUE_SIZE)
                        {
                            _logger.Warning($"Message queue for sending to socket {socketId} is full. The messages in queue will be cleared.");
                            queue.Clear();
                        }

                        if (queue.TryDequeue(out Message? message) && message is not null)
                        {
                            _sockets[socketId].Send(message.Json);
                            _logger.Debug($"Sent message \"{message.MessageType}\" to socket {socketId}.");
                        }
                        else
                        {
                            Task.Delay(MESSAGE_SENDING_INTERVAL).Wait();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Failed to send message to socket {socketId}: {ex.Message}");
                    _logger.Debug($"{ex}");
                }
            }
        });
    }
}

namespace GameServer.Connection;

public partial class AgentServer
{
    private Task? _taskForPublishingMessage = null;

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

    private void ActionForPublishingMessage()
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
                Task.Delay(MESSAGE_PUBLISH_INTERVAL).Wait();
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
    }
}

using System.Collections.Concurrent;
using Fleck;

namespace GameServer.Connection;

public partial class AgentServer
{
    private void AddSocket(Guid socketId, IWebSocketConnection socket)
    {
        try
        {
            _sockets.TryAdd(socketId, socket);

            _socketRawTextReceivingQueue.AddOrUpdate(
                socketId,
                new ConcurrentQueue<string>(),
                (key, oldValue) => new ConcurrentQueue<string>()
            );

            _socketMessageSendingQueue.AddOrUpdate(
                socketId,
                new ConcurrentQueue<Message>(),
                (key, oldValue) => new ConcurrentQueue<Message>()
            );

            // Cancel the previous task if it exists
            _ctsForParsingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForParsingMessage);
            ctsForParsingMessage?.Cancel();
            _ctsForParsingMessage.TryRemove(socketId, out _);

            _ctsForSendingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForSendingMessage);
            ctsForSendingMessage?.Cancel();
            _ctsForSendingMessage.TryRemove(socketId, out _);

            // Create new tasks for parsing and sending messages
            Task parsingTask = CreateTaskForParsingMessage(socketId);
            parsingTask.Start();

            _tasksForParsingMessage.AddOrUpdate(
                socket.ConnectionInfo.Id,
                parsingTask,
                (key, oldValue) =>
                {
                    oldValue?.Dispose();
                    return parsingTask;
                }
            );

            Task sendingTask = CreateTaskForSendingMessage(socketId);
            sendingTask.Start();

            _tasksForSendingMessage.AddOrUpdate(
                socketId,
                sendingTask,
                (key, oldValue) =>
                {
                    oldValue?.Dispose();
                    return sendingTask;
                }
            );
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to add {GetAddress(socket)}: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private void RemoveSocket(Guid socketId)
    {
        try
        {
            _sockets.TryRemove(socketId, out _);
            _socketTokens.TryRemove(socketId, out _);

            _ctsForParsingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForParsingMessage);
            ctsForParsingMessage?.Cancel();
            _ctsForParsingMessage.TryRemove(socketId, out _);

            _ctsForSendingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForSendingMessage);
            ctsForSendingMessage?.Cancel();
            _ctsForSendingMessage.TryRemove(socketId, out _);

            _tasksForParsingMessage.TryRemove(socketId, out _);
            _tasksForSendingMessage.TryRemove(socketId, out _);

            _socketMessageSendingQueue.TryRemove(socketId, out _);
            _socketRawTextReceivingQueue.TryRemove(socketId, out _);
        }
        catch (Exception ex)
        {
            _logger.Error($"Failed to remove {GetAddress(socketId)}: {ex.Message}");
            _logger.Debug($"{ex}");
        }
    }

    private string GetAddress(IWebSocketConnection socket)
    {
        try
        {
            return $"{socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort}";
        }
        catch (Exception)
        {
            return "[UNKNOWN]";
        }
    }

    private string GetAddress(Guid socketId)
    {
        try
        {
            if (_sockets.TryGetValue(socketId, out IWebSocketConnection? socket) && socket is not null)
            {
                return GetAddress(socket);
            }
            else
            {
                return "[UNKNOWN]";
            }
        }
        catch (Exception)
        {
            return "[UNKNOWN]";
        }
    }
}

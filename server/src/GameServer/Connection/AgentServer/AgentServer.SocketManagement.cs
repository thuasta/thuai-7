using System.Collections.Concurrent;
using Fleck;

namespace GameServer.Connection;

public partial class AgentServer
{
    private void AddSocket(Guid socketId, IWebSocketConnection socket)
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

    private void RemoveSocket(Guid socketId)
    {
        _sockets.TryRemove(socketId, out _);
        _socketTokens.TryRemove(socketId, out _);

        _ctsForParsingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForParsingMessage);
        ctsForParsingMessage?.Cancel();
        _ctsForParsingMessage.TryRemove(socketId, out _);

        _ctsForSendingMessage.TryGetValue(socketId, out CancellationTokenSource? ctsForSendingMessage);
        ctsForSendingMessage?.Cancel();
        _ctsForSendingMessage.TryRemove(socketId, out _);

        _tasksForParsingMessage.TryGetValue(socketId, out Task? taskForParsingMessage);
        taskForParsingMessage?.Dispose();
        _tasksForParsingMessage.TryRemove(socketId, out _);

        _tasksForSendingMessage.TryGetValue(socketId, out Task? taskForSendingMessage);
        taskForSendingMessage?.Dispose();
        _tasksForSendingMessage.TryRemove(socketId, out _);

        _socketMessageSendingQueue.TryRemove(socketId, out _);
        _socketRawTextReceivingQueue.TryRemove(socketId, out _);
    }

    private string GetAddress(IWebSocketConnection socket)
    {
        return $"{socket.ConnectionInfo.ClientIpAddress}: {socket.ConnectionInfo.ClientPort}";
    }

    private string GetAddress(Guid socketId)
    {
        if (_sockets.TryGetValue(socketId, out IWebSocketConnection? socket) && socket is not null)
        {
            return GetAddress(socket);
        }
        else
        {
            return "UNKNOWN";
        }
    }
}

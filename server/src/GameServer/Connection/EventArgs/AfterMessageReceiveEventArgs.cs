namespace GameServer.Connection;

public class AfterMessageReceiveEventArgs : EventArgs
{
    /// <summary>
    /// The message received
    /// </summary>
    public Message Message { get; }
    public Guid SocketId { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message received</param>
    public AfterMessageReceiveEventArgs(Message message, Guid socketId)
    {
        Message = message;
        SocketId = socketId;
    }
}

namespace GameServer.Connection;

public class AfterMessageReceiveEventArgs : EventArgs
{
    /// <summary>
    /// The message received
    /// </summary>
    public Message Message { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="message">The message received</param>
    public AfterMessageReceiveEventArgs(Message message)
    {
        Message = message;
    }
}

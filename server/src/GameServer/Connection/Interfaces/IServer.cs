namespace GameServer.Connection;

public interface IServer
{
    /// <summary>
    /// The number of messages published per second
    /// </summary>
    public const float MessagesPublishedPerSecond = 20;

    /// <summary>
    /// Event raised after a message is received
    /// </summary>
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    /// <summary>
    /// IP address of the server
    /// </summary>
    public string IpAddress { get; }

    /// <summary>
    /// Port of the server
    /// </summary>
    public int Port { get; }

    public Task? TaskForPublishingMessage { get; }

    /// <summary>
    /// Start the server
    /// </summary>
    public void Start();

    /// <summary>
    /// Stop the server
    /// </summary>
    public void Stop();

    /// <summary>
    /// Publish message to all clients
    /// </summary>
    /// <param name="message">The message to publish</param>
    public void Publish(Message message);
}

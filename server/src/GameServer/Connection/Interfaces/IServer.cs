namespace GameServer.Connection;

public interface IServer
{
    /// <summary>
    /// Interval between two message publishings. In milliseconds.
    /// </summary>
    public const int MessagePublishIntervalMilliseconds = 10;

    /// <summary>
    /// Event raised after a message is received
    /// </summary>
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    /// <summary>
    /// IP address of the server
    /// </summary>
    public string IpAddress { get; init; }

    /// <summary>
    /// Port of the server
    /// </summary>
    public int Port { get; init; }

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

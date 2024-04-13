namespace GameServer.Connection;

public class ClientServer : IServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent = delegate { };

    public string IpAddress { get; init; } = "0.0.0.0";
    public int Port { get; init; } = 8100;
    public Task? TaskForPublishingMessage => throw new NotImplementedException();

    public void Start() => throw new NotImplementedException();

    public void Stop() => throw new NotImplementedException();

    public void Publish(Message message) => throw new NotImplementedException();
}

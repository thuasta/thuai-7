namespace GameServer.Connction;

public class ClientServer : IServer
{
    public event EventHandler<AfterMessageReceiveEventArgs>? AfterMessageReceiveEvent;

    public string IpAddress { get; } = "0.0.0.0";
    public int Port { get; } = 8100;
    public Task? TaskForPublishingMessage => throw new NotImplementedException();

    public void Start() => throw new NotImplementedException();

    public void Stop() => throw new NotImplementedException();

    public void Publish(Message message) => throw new NotImplementedException();
}

namespace GameServer.GameController;

public class AfterPlayerConnect : EventArgs
{
    public int PlayerId { get; }
    public string Token { get; }
    public Guid SocketId { get; }

    public AfterPlayerConnect(int playerId, string token, Guid socketId)
    {
        PlayerId = playerId;
        Token = token;
        SocketId = socketId;
    }
}

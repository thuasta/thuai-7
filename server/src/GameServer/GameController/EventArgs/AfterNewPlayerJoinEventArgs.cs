namespace GameServer.GameController;

public class AfterNewPlayerJoinEventArgs : EventArgs
{
    public int PlayerId { get; }
    public string Token { get; }
    public Guid SocketId { get; }

    public AfterNewPlayerJoinEventArgs(int playerId, string token, Guid socketId)
    {
        PlayerId = playerId;
        Token = token;
        SocketId = socketId;
    }
}

namespace GameServer.GameLogic;

public class PlayerUseGrenadeEventArgs : EventArgs
{
    public const string EventName = "PlayerUseGrenade";
    public int PlayerId { get; }
    public Position TargetPosition { get; }

    public PlayerUseGrenadeEventArgs(int playerId, Position targetPosition)
    {
        PlayerId = playerId;
        TargetPosition = targetPosition;
    }

}
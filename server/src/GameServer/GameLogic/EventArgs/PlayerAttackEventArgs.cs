namespace GameServer.GameLogic;

public class PlayerAttackEventArgs : EventArgs
{
    public const string EventName = "PlayerAttack";
    public int PlayerId { get; }
    public Position TargetPosition { get; }

    public PlayerAttackEventArgs(int playerId, Position targetPosition)
    {
        PlayerId = playerId;
        TargetPosition = targetPosition;
    }

}
